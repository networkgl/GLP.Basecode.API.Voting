using System.ComponentModel.DataAnnotations;

namespace GLP.Basecode.API.Model.ApiModel
{
    public class ForgotPasswordViewInputModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string UserEmail { get; set; } = null!;
    }
}
