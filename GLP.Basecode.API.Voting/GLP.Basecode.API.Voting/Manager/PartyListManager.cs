using GLP.Basecode.API.Voting.Models;
using GLP.Basecode.API.Voting.Repository;
using GLP.Basecode.API.Voting.Constant;
using GLP.Basecode.API.Voting.Models.CustomModel;
using GLP.Basecode.API.Voting.Handler;
using GLP.Basecode.API.Voting.Services;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.EntityFrameworkCore;

namespace GLP.Basecode.API.Voting.Manager
{
    public class PartyListManager
    {
        private readonly VotingAppDbContext _dbContext;
        private readonly BaseRepository<PartyList> _partyListRepo;
        private readonly BaseRepository<FilePath> _filePathRepo;
        private readonly ImageFilePath _imageFilePath;

        public PartyListManager(
            VotingAppDbContext dbContext,
            BaseRepository<PartyList> partyListRepo,
            BaseRepository<FilePath> filePathRepo,
            ImageFilePath imageFilePath
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
            var path = _imageFilePath.SaveImageInFolder(imageBytes, schoolYear, rootFolder, model.PartyListName);

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {

                var newFilePath = new FilePath()
                {
                    Path = path
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

    }
}
