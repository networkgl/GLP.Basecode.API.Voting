using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GLP.Basecode.API.Voting.Models.CustomModel;
using GLP.Basecode.API.Voting.Manager;
using System.Threading.Tasks;
using GLP.Basecode.API.Voting.Constant;

namespace GLP.Basecode.API.Voting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountManager _accManager;

        public AccountController(AccountManager accManager)
        {
            _accManager = accManager;
        }

        [HttpPost("/create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountViewInputModel model)
        {
            var retVal = await _accManager.CreateStudentAccount(model);

            return retVal.Result switch
            {
                AccountCreationResult.Success => Ok(new { success = true, message = retVal.Message }),
                AccountCreationResult.DuplicateIdNumber => Conflict(new { success = false, message = retVal.Message }),
                AccountCreationResult.Error => StatusCode(500, new { success = false, message = retVal.Message }),
                _ => StatusCode(500, new { success = false, message = "Unknown error occurred." })
            };
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewInputModel model)
        {
            var retVal = await _accManager.CheckUserCredentials(model);

            return retVal.Result switch
            {
                LoginResult.Success => Ok(new { success = true, message = retVal.Message }),
                LoginResult.UserNotFound => NotFound(new { success = false, message = retVal.Message }),
                LoginResult.InvalidPassword => Unauthorized(new { success = false, message = retVal.Message }),
                _ => StatusCode(500, new { success = false, message = "Unknown error occurred." })
            };
        }

        [HttpPost("/recovery/send-otp")]
        public async Task<IActionResult> SendOTPForgotPassword([FromBody] ForgotPasswordViewInputModel model)
        {
            var result = await _accManager.SendOTPForgotPassword(model);

            return result.Status switch
            {
                ErrorCode.Success => Ok(new { success = true, message = result.SuccessMessage }),
                ErrorCode.NotFound => NotFound(new { success = false, message = result.ErrorMessage }),
                _ => StatusCode(500, new { success = false, message = result.ErrorMessage })
            };
        }

        [HttpGet("/getAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var retVal = await _accManager.GetAllRoles();
            return Ok(new { data = retVal, message = "Data successfully retrieve." });
        }
    }
       
}
