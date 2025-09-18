using GLP.Basecode.API.DAL.DAC.Interfaces;
using GLP.Basecode.API.Model.Enum;
using GLP.Basecode.API.Model.ApiModel;
using GLP.Basecode.API.Model;
using GLP.Basecode.API.Helper;
using GLP.Basecode.API.DAL.DAC.Repository;
using Microsoft.EntityFrameworkCore;
using GLP.Basecode.API.DAL.Data;

namespace GLP.Basecode.API.BLL.Managers
{
    public class CandidateManager
    {
        private readonly VotingContext _dbContext;
        private readonly BaseRepository<Student> _studentRepo;
        private readonly BaseRepository<Candidate> _candidateRepo;
        private readonly BaseRepository<FilePath> _filePathRepo;
        private readonly BaseRepository<PartyList> _partyListRepo;
        private readonly BaseRepository<CandidatePosition> _canPostRepo;
        private readonly BaseRepository<Position> _positionRepo;
        private readonly ImageFileHelper _imageFilePath;
        private readonly ExceptionMessageHelper _errMsg;

        public CandidateManager(
            VotingContext dbContext,
            BaseRepository<Student> studentRepo,
            BaseRepository<Candidate> candidateRepo,
            BaseRepository<FilePath> filePathRepo,
            BaseRepository<PartyList> partyListRepo,
            BaseRepository<CandidatePosition> canPosRepo,
            BaseRepository<Position> positionRepo,
            ImageFileHelper imageFilePath,
            ExceptionMessageHelper errMsg
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

        //tested
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

        //tested
        public async Task<OperationResult<ErrorCode>> UpdateCandidate(long canId, long ptylstId, UpdateCandidateViewModel model)
        {
            var opRes = new OperationResult<ErrorCode>();

            if (model.CandidateImage is null && model.NewPositionId is null)
            {
                opRes.SuccessMessage = "Candidate Image and Position Id cannot be null.";
                opRes.Status = ErrorCode.Error;
                return opRes;
            }

            var candidateDetails = await _dbContext.VwGetCandidateOrPartylistDetails.Where(c => c.CandidateId == canId && c.PartyListId == ptylstId).FirstOrDefaultAsync();
            if (candidateDetails is null)
            {
                opRes.ErrorMessage = $"Error: No candidate found with the ID: {canId}.";
                opRes.Status = ErrorCode.Error;
                return opRes;
            }

            //Needs to check if the position has not been acquried by other candidates.
            //But check first for nullability since user can update img OR position
            if (model.NewPositionId is not null && candidateDetails.PositionId == model.NewPositionId)
            {
                opRes.ErrorMessage = $"Error: There is already candidate associated with the position you've selected.";
                opRes.Status = ErrorCode.BadRequest;
                return opRes;
            }

            var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                //1. Image Update
                if (model.CandidateImage is not null)
                {
                    var isDeleted = _imageFilePath.DeleteImage(candidateDetails.CandidateImgPath);
                    if (!isDeleted)
                    {
                        opRes.ErrorMessage = $"Error: Cannot delete image from path: {candidateDetails.CandidateImgPath}";
                        opRes.Status = ErrorCode.Error;
                        return opRes;
                    }

                    //Handle file paths
                    var imageBytes = _imageFilePath.SaveAsPNG(model.CandidateImage);
                    var schoolYear = (DateTime.UtcNow.Year - 1).ToString() + "-" + DateTime.UtcNow.Year.ToString();
                    string rootFolder = "Party List";
                    string partyListName = candidateDetails.PartyListName;
                    var availablePos = await _positionRepo.GetAsyncById(model.NewPositionId);
                    var NewPosition = availablePos.Data is null ? "No position" : availablePos.Data.PositionName;
                    string candidateName = candidateDetails.CandidateName;

                    var (isSaved, newCanImgPth, errMsg) = _imageFilePath.SaveImageToCandidateFolder(imageBytes, schoolYear, rootFolder, partyListName, candidateName, NewPosition);
                    if (!isSaved)
                    {
                        opRes.ErrorMessage = errMsg;
                        opRes.Status = ErrorCode.Error;
                        return opRes;
                    }

                    //Assign new path.
                    var filePath = await _filePathRepo.GetAsyncById(candidateDetails.FilePathId);
                    filePath.Data.Path = newCanImgPth;

                    var retValNewCanImg = await _filePathRepo.UpdateAsync(filePath.Data.FilePathId, filePath.Data);
                    if (retValNewCanImg.Status == ErrorCode.Error)
                    {
                        await transaction.RollbackAsync();
                        opRes.ErrorMessage = retValNewCanImg.ErrorMessage;
                        opRes.Status = retValNewCanImg.Status;
                        return opRes;
                    }
                }

                //2. Position Update
                if (model.NewPositionId is not null)
                {
                    //Handle file paths
                    var schoolYear = (DateTime.UtcNow.Year - 1).ToString() + "-" + DateTime.UtcNow.Year.ToString();
                    string rootFolder = "Party List";
                    string partyListName = candidateDetails.PartyListName;
                    var availablePos = await _positionRepo.GetAsyncById(model.NewPositionId);
                    var NewPosition = availablePos.Data is null ? "No position" : availablePos.Data.PositionName;
                    string candidateName = candidateDetails.CandidateName;

                    //Rename img file if user did not update img else do nothing...
                    if (model.CandidateImage is null)
                    {
                        var (isSaved, newCanImgPath, errMsg) = _imageFilePath.RenameCanImgFileName(schoolYear, rootFolder, partyListName, candidateDetails.CandidateImgPath, candidateName, NewPosition);
                        if (!isSaved)
                        {
                            opRes.ErrorMessage = errMsg;
                            opRes.Status = ErrorCode.Error;
                            return opRes;
                        }

                        //Assign new path.
                        var filePath = await _filePathRepo.GetAsyncById(candidateDetails.FilePathId);
                        filePath.Data.Path = newCanImgPath;

                        var retValNewCanImg = await _filePathRepo.UpdateAsync(filePath.Data.FilePathId, filePath.Data);
                        if (retValNewCanImg.Status == ErrorCode.Error)
                        {
                            await transaction.RollbackAsync();
                            opRes.ErrorMessage = retValNewCanImg.ErrorMessage;
                            opRes.Status = retValNewCanImg.Status;
                            return opRes;
                        }
                    }

                    var canPos = await _canPostRepo.GetAsyncById(candidateDetails.CandidateId);
                    canPos.Data.PositionId = (long)model.NewPositionId;

                    var retValCanPos = await _canPostRepo.UpdateAsync(canPos.Data.CanposId, canPos.Data);
                    if (retValCanPos.Status == ErrorCode.Error)
                    {
                        await transaction.RollbackAsync();
                        opRes.ErrorMessage = retValCanPos.ErrorMessage;
                        opRes.Status = retValCanPos.Status;
                        return opRes;
                    }
                }


                if (model.CandidateImage is not null && model.NewPositionId is not null)
                    opRes.SuccessMessage = "Candidate image and position successfully udpated.";
                else if (model.NewPositionId is not null)
                    opRes.SuccessMessage = "Candidate position successfully udpated.";
                else
                    opRes.SuccessMessage = "Candidate image successfully udpated.";


                await transaction.CommitAsync();

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
