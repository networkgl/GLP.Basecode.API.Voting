using System.ComponentModel.DataAnnotations;

namespace GLP.Basecode.API.Voting.Models.CustomModel
{
    public class CreateAccountViewInputModel
    {
        //Student
        [Required(ErrorMessage = "ID number is required.")]
        [Range(10000000, 99999999, ErrorMessage = "ID number must be exactly 8 digits.")]
        public int IdNumber { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = null!;

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public long CourseId { get; set; }

        [Required(ErrorMessage = "School Year ID is required.")]
        public long SyId { get; set; }


        //User
        [Required(ErrorMessage = "Email is required.")]
        public string UserEmail { get; set; } = null!;

    }
}
