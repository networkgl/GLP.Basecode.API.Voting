using GLP.Basecode.API.Voting.Models;
using GLP.Basecode.API.Voting.Repository;
using GLP.Basecode.API.Voting.Constant;
using GLP.Basecode.API.Voting.Models.CustomModel;
using GLP.Basecode.API.Voting.Handler;
using GLP.Basecode.API.Voting.Services;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace GLP.Basecode.API.Voting.Manager
{
    public class PartyListManager
    {
        private readonly VotingAppDbContext _dbContext;
        private readonly BaseRepository<PartyList> _partyListRepo;
        private readonly BaseRepository<FilePath> _filePathRepo;
        private readonly PartyListImageFileManager _imageFilePath;

        public PartyListManager(
            VotingAppDbContext dbContext,
            BaseRepository<PartyList> partyListRepo,
            BaseRepository<FilePath> filePathRepo,
            PartyListImageFileManager imageFilePath
            )
        {
            _dbContext = dbContext;
            _partyListRepo = partyListRepo;
            _filePathRepo = filePathRepo;
            _imageFilePath = imageFilePath;
        }

        //tested
        public async Task<OperationResult<PartyList?>> GetPartyListById(long id)
        {
            return await _partyListRepo.GetAsyncById(id);
        }

        //tested
        public async Task<OperationResult<ErrorCode>> CreatePartyList(CreatePartyListViewInputModel model)
        {
            var opRes = new OperationResult<ErrorCode>();
            
            var hasExistedPartyList = await _partyListRepo.FindAsyncByPredicate(p => p.PartyListName.Trim() == model.PartyListName.Trim());
            if (hasExistedPartyList is not null)
            {
                opRes.Status = ErrorCode.Duplicate;
                opRes.ErrorMessage = OperationResultMessageResponse.DUPLICATE; //PARTYLIST EXIST
                return opRes;
            }

            //Handle file paths
            var imageBytes = _imageFilePath.SaveAsPNG(model.PartyListImage);
            var schoolYear = (DateTime.UtcNow.Year - 1).ToString() + "-" + DateTime.UtcNow.Year.ToString();
            string rootFolder = "Party List";
            var imgPath = _imageFilePath.SaveImageInFolderAsCreatePartyList(imageBytes, schoolYear, rootFolder, model.PartyListName.Trim());

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {

                var newFilePath = new FilePath()
                {
                    Path = imgPath
                };

                var retValFilePath = await _filePathRepo.CreateAsync(newFilePath);
                if (retValFilePath.Status == ErrorCode.Error)
                {
                    opRes.Status = ErrorCode.Error;
                    opRes.ErrorMessage = retValFilePath.ErrorMessage;
                    await transaction.RollbackAsync();
                    return opRes;
                }

                var newPartyList = new PartyList()
                {
                    PartyListName = model.PartyListName,
                    FilePathId = newFilePath.FilePathId,
                    CreatedAt = TimeZoneConverter.ConvertTimeZone(DateTime.UtcNow)
                };

                var retValPartyList = await _partyListRepo.CreateAsync(newPartyList);
                if (retValPartyList.Status == ErrorCode.Error)
                {
                    opRes.Status = ErrorCode.Error;
                    opRes.ErrorMessage = retValPartyList.ErrorMessage;
                    await transaction.RollbackAsync();
                    return opRes;
                }

                await transaction.CommitAsync();

                opRes.Status = ErrorCode.Success;
                opRes.SuccessMessage = "Party List successfully added.";

                return opRes;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();

                var _errMsg = new ExceptionHandlerMessage(); ;
                opRes.ErrorMessage = $"Transaction failed: {_errMsg.GetInnermostExceptionMessage(e)}";
                opRes.Status = ErrorCode.Error;
                return opRes;
            }
        }


        public async Task<OperationResult<ErrorCode>> EditPartyList(long id, UpdatePartyListViewModel model)
        {
            var result = new OperationResult<ErrorCode>();

            if (model.PartyListName == null && model.PartyListImage == null)
            {
                result.ErrorMessage = "Party List name and campaign image cannot be null.";
                result.Status = ErrorCode.Error;
                return result;
            }

            var partyList = await _partyListRepo.GetAsyncById(id);
            if (partyList.Data == null)
            {
                result.ErrorMessage = partyList.ErrorMessage;
                result.Status = partyList.Status;
                return result;
            }

            var filePath = await _filePathRepo.GetAsyncById(partyList.Data.FilePathId);
            if (filePath.Data == null)
            {
                result.ErrorMessage = "File path record not found.";
                result.Status = ErrorCode.Error;
                return result;
            }

            var schoolYear = $"{DateTime.UtcNow.Year - 1}-{DateTime.UtcNow.Year}";
            const string rootFolder = "Party List";

            bool nameChanged = false;
            string? renamedFolderPath = null;
            string? renamedImageName = null;

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Update Party List Name
                if (!string.IsNullOrWhiteSpace(model.PartyListName))
                {
                    nameChanged = true;
                    var oldName = partyList.Data.PartyListName;
                    partyList.Data.PartyListName = model.PartyListName.Trim();

                    var updatePartyList = await _partyListRepo.UpdateAsync(partyList.Data.PartyListId, partyList.Data);
                    if (updatePartyList.Status == ErrorCode.Error)
                    {
                        await transaction.RollbackAsync();
                        return new OperationResult<ErrorCode> { ErrorMessage = updatePartyList.ErrorMessage, Status = ErrorCode.Error };
                    }

                    // Rename folder
                    var (folderRenamed, newPath, fileName, errMsg) = _imageFilePath.RenameFolder(schoolYear, rootFolder, oldName, partyList.Data.PartyListName);
                    if (!folderRenamed)
                    {
                        await transaction.RollbackAsync();
                        result.ErrorMessage = errMsg;
                        result.Status = ErrorCode.Error;
                        return result;
                    }

                    renamedFolderPath = newPath;
                    renamedImageName = fileName;

                    // If no new image uploaded, just update the path in DB
                    if (model.PartyListImage == null)
                    {
                        filePath.Data.Path = $"{newPath}/{fileName}";
                        var updateFilePath = await _filePathRepo.UpdateAsync(filePath.Data.FilePathId, filePath.Data);

                        if (updateFilePath.Status == ErrorCode.Error)
                        {
                            await transaction.RollbackAsync();
                            return new OperationResult<ErrorCode> { ErrorMessage = updateFilePath.ErrorMessage, Status = ErrorCode.Error };
                        }

                        result.SuccessMessage = "Party List name updated successfully.";
                    }
                }

                // 2. Update Party List Image
                if (model.PartyListImage != null)
                {
                    string oldImagePath = nameChanged
                        ? $"{renamedFolderPath}/{renamedImageName}"
                        : filePath.Data.Path;

                    if (!_imageFilePath.DeleteImage(oldImagePath))
                    {
                        await transaction.RollbackAsync();
                        result.ErrorMessage = "Failed to remove the old image from the server.";
                        result.Status = ErrorCode.Error;
                        return result;
                    }

                    var imageBytes = _imageFilePath.SaveAsPNG(model.PartyListImage);
                    string? newImagePath;

                    if (nameChanged && renamedFolderPath != null)
                    {
                        var (saved, relativePath, errMsg) = _imageFilePath.SaveImageToFullPath(imageBytes, schoolYear, rootFolder, partyList.Data.PartyListName);
                        if (!saved)
                        {
                            await transaction.RollbackAsync();
                            result.ErrorMessage = errMsg;
                            result.Status = ErrorCode.Error;
                            return result;
                        }
                        newImagePath = relativePath;
                    }
                    else
                    {
                        newImagePath = _imageFilePath.SaveImageInFolderAsCreatePartyList(imageBytes, schoolYear, rootFolder, partyList.Data.PartyListName);
                    }

                    filePath.Data.Path = newImagePath;

                    var updateImagePath = await _filePathRepo.UpdateAsync(filePath.Data.FilePathId, filePath.Data);
                    if (updateImagePath.Status == ErrorCode.Error)
                    {
                        await transaction.RollbackAsync();
                        _imageFilePath.DeleteImage(newImagePath); // rollback file creation
                        return new OperationResult<ErrorCode> { ErrorMessage = updateImagePath.ErrorMessage, Status = ErrorCode.Error };
                    }

                    result.SuccessMessage = nameChanged
                        ? "Party List name and image successfully updated."
                        : "Party List image successfully updated.";
                }

                await transaction.CommitAsync();
                result.Status = ErrorCode.Success;
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResult<ErrorCode>
                {
                    ErrorMessage = $"An error occurred: {ex.Message}",
                    Status = ErrorCode.Error
                };
            }
        }

    }
}
