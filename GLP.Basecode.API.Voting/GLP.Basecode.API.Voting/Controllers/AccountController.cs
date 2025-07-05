using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GLP.Basecode.API.Voting.Models.CustomModel;
using GLP.Basecode.API.Voting.Manager;
using System.Threading.Tasks;
using GLP.Basecode.API.Voting.Constant;
using GLP.Basecode.API.Voting.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace GLP.Basecode.API.Voting.Controllers
{
    [Authorize(Roles = "SBO Admin,Student")]
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly AccountManager _accManager;
        private readonly JwtSettings _jwtSettings;

        public AccountController(AccountManager accManager, IOptions<JwtSettings> jwtSettings)
        {
            _accManager = accManager;
            _jwtSettings = jwtSettings.Value;
        }

        //tested
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewInputModel model)
        {
            var retVal = await _accManager.CheckUserCredentials(model);

            if (retVal.Result != LoginResult.Success)
            {
                return retVal.Result switch
                {
                    LoginResult.UserNotFound => NotFound(new { success = false, message = retVal.Message }),
                    LoginResult.InvalidPassword => Unauthorized(new { success = false, message = retVal.Message }),
                    _ => StatusCode(500, new { success = false, message = "Unknown error occurred." })
                };
            }

            var user = await _accManager.GetUserByUsername(model.Username);
            if (user is null)
                return NotFound(new { success = false, message = retVal.Message });


            // Build token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.RoleName) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            //var a = TimeZoneConverter.ConvertTimeZone(DateTime.UtcNow);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                success = true,
                message = retVal.Message,
                token = tokenString
            });
        }

        //tested
        [Authorize(Roles = "SBO Admin")]
        [HttpGet("getAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var retVal = await _accManager.GetAllRoles();
            return Ok(new { data = retVal, message = "Data successfully retrieve." });
        }

        //tested
        [Authorize(Roles = "SBO Admin")]
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountViewInputModel model)
        {
            var retVal = await _accManager.CreateStudentAccount(model);

            return retVal.Result switch
            {
                AccountCreationResult.Success => Ok(new { success = true, message = retVal.Message }),
                AccountCreationResult.DuplicateIdNumber => Conflict(new { success = false, message = retVal.Message }),
                AccountCreationResult.DuplicateEmail => Conflict(new { success = false, message = retVal.Message }),
                AccountCreationResult.Error => StatusCode(500, new { success = false, message = retVal.Message }),
                _ => StatusCode(500, new { success = false, message = "Unknown error occurred." })
            };
        }

        //tested
        [AllowAnonymous] 
        [HttpPost("recovery/send-otp")]
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


    }
       
}
