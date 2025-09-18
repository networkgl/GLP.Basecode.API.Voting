using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GLP.Basecode.API.BLL.Managers;
using GLP.Basecode.API.BLL.Services;
using GLP.Basecode.API.Model.Enum;
using GLP.Basecode.API.Model.ApiModel;
using System.Threading.Tasks;
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

            if (retVal.Status != ErrorCode.Success)
            {
                return retVal.Status switch
                {
                    ErrorCode.NotFound => NotFound(new { success = false, message = retVal.ErrorMessage }),
                    ErrorCode.Error => Unauthorized(new { success = false, message = retVal.ErrorMessage }),
                    _ => StatusCode(500, new { success = false, message = "Unknown error occurred." })
                };
            }

            var user = await _accManager.GetUserByUsername(model.Username);
            if (user is null)
                return NotFound(new { success = false, message = retVal.ErrorMessage });


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
                message = retVal.SuccessMessage,
                token = tokenString
            });
        }

        //tested
        [Authorize(Roles = "SBO Admin")]
        [HttpGet("get-all-roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var retVal = await _accManager.GetAllRoles();
            return Ok(new { data = retVal, message = "Data successfully retrieve." });
        }

        //tested
        [Authorize(Roles = "SBO Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateStudentAccount([FromBody] CreateAccountViewInputModel model)
        {
            var retVal = await _accManager.CreateStudentAccount(model);

            return retVal.Status switch
            {
                ErrorCode.Success => Ok(new { success = true, message = retVal.ErrorMessage }),
                ErrorCode.Duplicate => Conflict(new { success = false, message = retVal.ErrorMessage }),
                ErrorCode.Error => StatusCode(500, new { success = false, message = retVal.ErrorMessage }),
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
