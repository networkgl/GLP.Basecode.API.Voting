using GLP.Basecode.API.Voting.Controllers;
using GLP.Basecode.API.Voting.Repository;
using GLP.Basecode.API.Voting.Constant;
using GLP.Basecode.API.Voting.Models.CustomModel;
using GLP.Basecode.API.Voting.Handler;
using Microsoft.EntityFrameworkCore;
using GLP.Basecode.API.Voting.Models;
using Microsoft.AspNetCore.Identity;
using GLP.Basecode.API.Voting.Services;

namespace GLP.Basecode.API.Voting.Manager
{
    public class AccountManager
    {
        private readonly VotingAppDbContext _dbContext;
        private readonly MailManager _mailManager;
        private readonly BaseRepository<User> _userRepo;
        private readonly BaseRepository<Student> _studentRepo;
        private readonly BaseRepository<Role> _userRoleRepo;

        public AccountManager(
               VotingAppDbContext dbContext,
               BaseRepository<User> userRepo,
               BaseRepository<Student> studentRepo,
               MailManager mailManager,
               BaseRepository<Role> userRole)
        {
            _dbContext = dbContext;
            _userRepo = userRepo;
            _studentRepo = studentRepo;
            _mailManager = mailManager;
            _userRoleRepo = userRole;
        }

        //tested
        public async Task<List<Role>> GetAllRoles()
        {
            return await _userRoleRepo.GetAllAsync();
        }

        //tested
        public async Task<UsersWithRoleModel?> GetUserByUsername(string username)
        {
            //return await _userRepo.FindAsyncByPredicate(u => u.Username == username);

            var retVal = await (from user in _dbContext.Users
                                join role in _dbContext.Roles on user.RoleId equals role.RoleId
                                where user.Username.Trim().ToLower() == username.Trim().ToLower()
                                select new UsersWithRoleModel
                                {
                                    UserId = user.UserId,
                                    RoleId = role.RoleId,
                                    Username = user.Username,
                                    RoleName = role.RoleName
                                }).FirstOrDefaultAsync();


            return retVal;
        }

        //tested
        public async Task<OperationResult<ErrorCode>> SendOTPForgotPassword(ForgotPasswordViewInputModel model)
        {
            var opRes = new OperationResult<ErrorCode>();
            var user = await _userRepo.FindAsyncByPredicate(u => u.UserEmail == model.UserEmail);
            if (user is null)
            {
                opRes.ErrorMessage = $"No user found for the email: {model.UserEmail}";
                opRes.Status = ErrorCode.NotFound;

                return opRes;
            }

            //SENDING EMAIL HAPPEN HERE...
            var otp = OTPGenerator.Generate().ToString(); 
            var subject = "Your OTP Code for Password Reset";
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "EmailTemplate.html");
            string body = File.ReadAllText(templatePath)
                              .Replace("{{OTP}}", otp)
                              .Replace("{{YEAR}}", TimeZoneConverter.ConvertTimeZone(DateTime.UtcNow).Year.ToString());

            //var body = $"<p>Hello,</p><p>Your OTP code is: <strong>{otp}</strong></p>";

            var (sent, message) = await _mailManager.SendEmailAsync(subject, body, model.UserEmail);

            if (!sent)
            {
                opRes.ErrorMessage = $"Failed to send email: {message}";
                opRes.Status = ErrorCode.Error;
                return opRes;
            }

            opRes.SuccessMessage = message;

            //Store OTP in DB using UPDATE
            user.UserOtp = int.Parse(otp);
            var userUpdate = await _userRepo.UpdateAsync(user.UserId, user);

            if (userUpdate.Status == ErrorCode.Error)
            {
                opRes.ErrorMessage = userUpdate.ErrorMessage;
                opRes.Status = ErrorCode.Error;

                return opRes;
            }


            opRes.SuccessMessage = message + MaskEmail.Mask(model.UserEmail); //
            opRes.Status = userUpdate.Status;

            return opRes;
        }

        //tested
        public async Task<LoginResultResponse> CheckUserCredentials(LoginViewInputModel model)
        {
            var logRes = new LoginResultResponse();
            var user = await _userRepo.FindAsyncByPredicate(u => u.Username == model.Username);

            if (user == null)
            {
                logRes.Result = LoginResult.UserNotFound;
                logRes.Message = LoginResultMessageResponse.USER_NOT_FOUND;

                return logRes;
            }

            var hasher = new PasswordHasher<User>();
            //var result = hasher.VerifyHashedPassword(user, user.Password, model.Password);

            //if (result == PasswordVerificationResult.Failed){
            //    logRes.Result = LoginResult.InvalidPassword;
            //    logRes.Message = LoginResultMessageResponse.INVALID_PASSWORD;

            //    return logRes;
            //}

            if (user.Password != model.Password)
            {
                logRes.Result = LoginResult.InvalidPassword;
                logRes.Message = LoginResultMessageResponse.INVALID_PASSWORD;

                return logRes;
            }

            //else Success
            logRes.Result = LoginResult.Success;
            logRes.Message = LoginResultMessageResponse.SUCCESSFULL;

            return logRes;
        }

        //tested
        public async Task<AccountCreationResponse> CreateStudentAccount(CreateAccountViewInputModel model)
        {
            var accRes = new AccountCreationResponse();

            // Check for duplicate ID Number
            var hasExistedStudentId = await _studentRepo.FindAsyncByPredicate(s => s.IdNumber == model.IdNumber);
            if (hasExistedStudentId is not null)
            {
                accRes.Message = AccountCreationMessageResponse.DUPLICATE_USER;
                accRes.Result = AccountCreationResult.DuplicateIdNumber;
                return accRes;
            }

            // Check for duplicate User Email
            var hasExistedUserEmail = await _userRepo.FindAsyncByPredicate(s => s.UserEmail == model.UserEmail);
            if (hasExistedUserEmail is not null)
            {
                accRes.Message = AccountCreationMessageResponse.DUPLICATE_USER_EMAIL;
                accRes.Result = AccountCreationResult.DuplicateEmail;
                return accRes;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var student = new Student
                {
                    IdNumber = model.IdNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    CourseId = model.CourseId,
                    SyId = model.SyId
                };

                var addStudentRetVal = await _studentRepo.CreateAsync(student);
                if (addStudentRetVal.Status == ErrorCode.Error)
                {
                    accRes.Message = addStudentRetVal.ErrorMessage ?? "Error while creating student.";
                    accRes.Result = AccountCreationResult.Error;
                    return accRes;
                }

                var user = new User
                {
                    Username = model.IdNumber.ToString(),
                    Password = model.IdNumber.ToString(), //Initially not hashed..
                    UserEmail = model.UserEmail,
                    StudentId = student.StudentId,
                    RoleId = (byte)RoleType.Student
                };

                var addUserRetVal = await _userRepo.CreateAsync(user);
                if (addUserRetVal.Status == ErrorCode.Error)
                {
                    await transaction.RollbackAsync(); // ← manually rollback

                    accRes.Message = addUserRetVal.ErrorMessage ?? "Error while creating user.";
                    accRes.Result = AccountCreationResult.Error;
                    return accRes;
                }

                await transaction.CommitAsync();

                accRes.Message = AccountCreationMessageResponse.STUDENT_CREATED_SUCCESSFULLY;
                accRes.Result = AccountCreationResult.Success;
                return accRes;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();

                var _errMsg = new ExceptionHandlerMessage(); ;
                accRes.Message = $"Transaction failed: {_errMsg.GetInnermostExceptionMessage(e)}";
                accRes.Result = AccountCreationResult.Error;
                return accRes;
            }
        }

        
    }
}
