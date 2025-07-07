using GLP.Basecode.API.Voting.Constant;
using GLP.Basecode.API.Voting.Handler;
using GLP.Basecode.API.Voting.Models;
using GLP.Basecode.API.Voting.Models.ApiModel;
using GLP.Basecode.API.Voting.Repository;
using GLP.Basecode.API.Voting.Services;
using Microsoft.EntityFrameworkCore;

namespace GLP.Basecode.API.Voting.Manager
{
    public class CandidateManager
    {
        private readonly VotingAppDbContext _dbContext;
        private readonly BaseRepository<Student> _studentRepo;
        private readonly BaseRepository<Candidate> _candidateRepo;
        private readonly BaseRepository<FilePath> _filePathRepo;
        private readonly BaseRepository<PartyList> _partyListRepo;
        private readonly BaseRepository<CandidatePosition> _canPostRepo;
        private readonly BaseRepository<Position> _positionRepo;
        private readonly ImageFileManager _imageFilePath;
        private readonly ExceptionHandlerMessage _errMsg;

        public CandidateManager(
            VotingAppDbContext dbContext,
            BaseRepository<Student> studentRepo,
            BaseRepository<Candidate> candidateRepo,
            BaseRepository<FilePath> filePathRepo,
            BaseRepository<PartyList> partyListRepo,
            BaseRepository<CandidatePosition> canPosRepo,
            BaseRepository<Position> positionRepo,
            ImageFileManager imageFilePath,
            ExceptionHandlerMessage errMsg
            )
        {
            _dbContext = dbContext;
            _studentRepo = studentRepo;
            _candidateRepo = candidateRepo;
            _filePathRepo = filePathRepo;
            _partyListRepo = partyListRepo;
            _canPostRepo = canPosRepo;
            _positionRepo = positionRepo;
            _imageFilePath = imageFilePath;
            _errMsg = errMsg;
        }

        //tested
        public async Task<OperationResult<VwGetAllCandidatesByCurrentSchoolYear?>> GetCandidateBy(long candidateId)
        {
            var opRes = new OperationResult<VwGetAllCandidatesByCurrentSchoolYear?>();

            var candidates = await _dbContext.VwGetAllCandidatesByCurrentSchoolYears.ToListAsync();
            var getCandidateById = candidates.Where(c => c.CandidateId == candidateId).FirstOrDefault();
            if (getCandidateById is null)
            {
                opRes.ErrorMessage = $"Candidate not found ID: {candidateId}";
                opRes.Data = null;
                opRes.Status = ErrorCode.NotFound;
                return opRes;
            }

            opRes.SuccessMessage = "Data successfully retrieved.";
            opRes.Data = getCandidateById;
            opRes.Status = ErrorCode.Success;
            return opRes;
        }

        //not yet tested
        public async Task<OperationResult<ErrorCode>> CreateCandidate(CreateCandidateViewInputModel model)
        {
            var opRes = new OperationResult<ErrorCode>();

            var student = await _studentRepo.GetAsyncById(model.StudentId);
            if (student.Data is null)
            {
                opRes.ErrorMessage = student.ErrorMessage;
                opRes.Status = student.Status;
                return opRes;
            }

            var partyList = await _partyListRepo.GetAsyncById(model.PartyListId);
            if (partyList.Data is null)
            {
                opRes.ErrorMessage = student.ErrorMessage;
                opRes.Status = student.Status;
                return opRes;
            }


            //Handle file paths
            var imageBytes = _imageFilePath.SaveAsPNG(model.CandidateImage);
            var schoolYear = (DateTime.UtcNow.Year - 1).ToString() + "-" + DateTime.UtcNow.Year.ToString();
            string rootFolder = "Party List";
            string partyListName = partyList.Data.PartyListName;
            var position = await _positionRepo.GetAsyncById(model.PositionId);
            string candidateName = student.Data.FirstName + " " + student.Data.LastName;

            var (isSaved, imgPath, errMsg) = _imageFilePath.SaveImageToCandidateFolder(imageBytes, schoolYear, rootFolder, partyListName, candidateName, position.Data.PositionName);
            if (!isSaved)
            {
                opRes.ErrorMessage = errMsg;
                opRes.Status = ErrorCode.Error;
                return opRes;
            }


            var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                //Insert FilePath
                var newImg = new FilePath()
                {
                    Path = imgPath
                };
                var retValNewImg = await _filePathRepo.CreateAsync(newImg);
                if (retValNewImg.Status == ErrorCode.Error)
                {
                    opRes.ErrorMessage = retValNewImg.ErrorMessage; 
                    opRes.Status = retValNewImg.Status;
                    return opRes;
                }

                //Insert Candidate
                var newCandidate = new Candidate()
                {
                   PartyListId = model.PartyListId,
                   FilePathId = newImg.FilePathId,
                   StudentId = model.StudentId
                };
                var retValnewCandidate = await _candidateRepo.CreateAsync(newCandidate);
                if (retValnewCandidate.Status == ErrorCode.Error)
                {
                    opRes.ErrorMessage = retValnewCandidate.ErrorMessage;
                    opRes.Status = retValnewCandidate.Status;
                    return opRes;
                }

                //Insert Candidate Position
                var newCanPos = new CandidatePosition()
                {
                    CandidateId = newCandidate.CandidateId,
                    PositionId = model.PositionId
                };
                var retValnewCanPos = await _canPostRepo.CreateAsync(newCanPos);
                if (retValnewCanPos.Status == ErrorCode.Error)
                {
                    opRes.ErrorMessage = retValnewCanPos.ErrorMessage;
                    opRes.Status = retValnewCanPos.Status;
                    return opRes;
                }


                await transaction.CommitAsync();
                opRes.SuccessMessage = "Candidate successfully added.";
                opRes.Status = ErrorCode.Success;

                return opRes;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                opRes.ErrorMessage = _errMsg.GetInnermostExceptionMessage(e);
                opRes.Status = ErrorCode.Error;
                return opRes;
            }

        }


    }
}
