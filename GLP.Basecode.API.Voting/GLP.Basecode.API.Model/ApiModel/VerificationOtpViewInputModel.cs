using System.ComponentModel.DataAnnotations;

namespace GLP.Basecode.API.Model.ApiModel
{
    public class VerificationOtpViewInputModel
    {
        [Required(ErrorMessage = "Please enter the six digit OTP.")]
        public string? OTP { get; set; }
    }
}
