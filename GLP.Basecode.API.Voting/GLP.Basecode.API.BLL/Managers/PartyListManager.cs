using GLP.Basecode.API.Model;
using GLP.Basecode.API.DAL.DAC.Repository;
using GLP.Basecode.API.DAL.DAC.Interfaces;
using GLP.Basecode.API.Helper;
using GLP.Basecode.API.Model.ApiModel;
using GLP.Basecode.API.Model.Enum;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;
using System.Transactions;
using GLP.Basecode.API.DAL.Data;

namespace GLP.Basecode.API.BLL.Managers
{
    public class PartyListManager
    {
        private readonly VotingContext _dbContext;
        private readonly BaseRepository<PartyList> _partyListRepo;
        private readonly BaseRepository<FilePath> _filePathRepo;
        private readonly ImageFileHelper _imageFileHelper;

        public PartyListManager(
            VotingContext dbContext,
            BaseRepository<PartyList> partyListRepo,
            BaseRepository<FilePath> filePathRepo,
            ImageFileHelper imageFileHelper
            )
        {
            _dbContext = dbContext;
            _partyListRepo = partyListRepo;
            _filePathRepo = filePathRepo;
            _imageFileHelper = imageFileHelper;
        }

        //tested
        public async Task<OperationResult<PartyList?>> GetPartyListById(long id)
        {
            var opRes = new OperationResult<PartyList?>();

            var partyList = await _partyListRepo.GetAsyncById(id);
            if (partyList.Data is null)
            {
                opRes.ErrorMessage = partyList.ErrorMessage;
                opRes.Data = null;
                opRes.Status = partyList.Status;
                return opRes;
            }

            opRes.SuccessMessage = "Data successfully retrieved";
            opRes.Data = partyList.Data;
            opRes.Status = ErrorCode.Success;
            return opRes;
        }

        //tested
        public async Task<List<PartyList>> GetAllPartyList()
        {
            return await _partyListRepo.GetAllAsync();
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
            var imageBytes = _imageFileHelper.SaveAsPNG(model.PartyListImage);
            var schoolYear = (DateTime.UtcNow.Year - 1).ToString() + "-" + DateTime.UtcNow.Year.ToString();
            string rootFolder = "Party List";
            string partListName = string.Join(" ", model.PartyListName.Trim(), rootFolder);

            var (isSaved, imgPath, errMsg) = _imageFileHelper.SaveImageToPartyListFolder(imageBytes, schoolYear, rootFolder, partListName);
            if (!isSaved)
            {
                opRes.ErrorMessage = errMsg;
                opRes.Status = ErrorCode.Error;
                return opRes;
            }

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
                    PartyListName = partListName,
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

                var _errMsg = new ExceptionMessageHelper(); ;
                opRes.ErrorMessage = $"Transaction failed: {_errMsg.GetInnermostExceptionMessage(e)}";
                opRes.Status = ErrorCode.Error;
                return opRes;
            }
        }

        //tested
        public async Task<OperationResult<ErrorCode>> UpdatePartyList(long id, UpdatePartyListViewModel model)
        {
            var opRes = new OperationResult<ErrorCode>();

            if (model.PartyListName == null && model.PartyListImage == null)
            {
                opRes.ErrorMessage = "Party List name and campaign image cannot be null.";
                opRes.Status = ErrorCode.Error;
                return opRes;
            }

            var partyList = await _partyListRepo.GetAsyncById(id);
            if (partyList.Data == null)
            {
                opRes.ErrorMessage = partyList.ErrorMessage;
                opRes.Status = partyList.Status;
                return opRes;
            }

            var filePath = await _filePathRepo.GetAsyncById(partyList.Data.FilePathId);
            if (filePath.Data == null)
            {
                opRes.ErrorMessage = "File path record not found.";
                opRes.Status = ErrorCode.Error;
                return opRes;
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
                    partyList.Data.PartyListName = string.Join(" ", model.PartyListName.Trim(), rootFolder);

                    var updatePartyList = await _partyListRepo.UpdateAsync(partyList.Data.PartyListId, partyList.Data);
                    if (updatePartyList.Status == ErrorCode.Error)
                    {
                        await transaction.RollbackAsync();
                        return new OperationResult<ErrorCode> { ErrorMessage = updatePartyList.ErrorMessage, Status = ErrorCode.Error };
                    }

                    // Rename folder
                    var (folderRenamed, newPath, fileName, errMsg) = _imageFileHelper.RenameFolderPartyList(schoolYear, rootFolder, oldName, partyList.Data.PartyListName);
                    if (!folderRenamed)
                    {
                        await transaction.RollbackAsync();
                        opRes.ErrorMessage = errMsg;
                        opRes.Status = ErrorCode.Error;
                        return opRes;
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

                        opRes.SuccessMessage = "Party List name updated successfully.";
                    }
                }

                // 2. Update Party List Image
                if (model.PartyListImage != null)
                {
                    string oldImagePath = nameChanged
                        ? $"{renamedFolderPath}/{renamedImageName}"
                        : filePath.Data.Path;

                    if (!_imageFileHelper.DeleteImage(oldImagePath))
                    {
                        await transaction.RollbackAsync();
                        opRes.ErrorMessage = "Failed to remove the old image from the server.";
                        opRes.Status = ErrorCode.Error;
                        return opRes;
                    }

                    var imageBytes = _imageFileHelper.SaveAsPNG(model.PartyListImage);
                    string? newImagePath;


                    var (isSaved, relativePath, errMsg) = _imageFileHelper.SaveImageToPartyListFolder(imageBytes, schoolYear, rootFolder, partyList.Data.PartyListName);
                    if (!isSaved)
                    {
                        await transaction.RollbackAsync();
                        opRes.ErrorMessage = errMsg;
                        opRes.Status = ErrorCode.Error;
                        return opRes;
                    }
                    newImagePath = relativePath;

                    filePath.Data.Path = newImagePath;

                    var updateImagePath = await _filePathRepo.UpdateAsync(filePath.Data.FilePathId, filePath.Data);
                    if (updateImagePath.Status == ErrorCode.Error)
                    {
                        await transaction.RollbackAsync();
                        _imageFileHelper.DeleteImage(newImagePath); // rollback file creation
                        return new OperationResult<ErrorCode> { ErrorMessage = updateImagePath.ErrorMessage, Status = ErrorCode.Error };
                    }

                    opRes.SuccessMessage = nameChanged
                        ? "Party List name and image successfully updated."
                        : "Party List image successfully updated.";
                }

                await transaction.CommitAsync();
                opRes.Status = ErrorCode.Success;
                return opRes;
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
