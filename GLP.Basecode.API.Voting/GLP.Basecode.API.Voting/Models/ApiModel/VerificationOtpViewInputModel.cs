using System.ComponentModel.DataAnnotations;

namespace GLP.Basecode.API.Voting.Models.CustomModel
{
    public class VerificationOtpViewInputModel
    {
        [Required(ErrorMessage = "Please enter the six digit OTP.")]
        public string? OTP { get; set; }
    }
}
