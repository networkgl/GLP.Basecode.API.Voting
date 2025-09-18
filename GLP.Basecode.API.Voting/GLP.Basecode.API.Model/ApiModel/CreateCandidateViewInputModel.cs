using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GLP.Basecode.API.Model.ApiModel
{
    public class CreateCandidateViewInputModel
    {
        [Required(ErrorMessage = "Candidate image cannot be empty!")]
        public IFormFile CandidateImage { get; set; } = null!;

        [Required(ErrorMessage = "Student ID is null.")]
        public long StudentId { get; set; }

        [Required(ErrorMessage = "PartyList ID is null.")]
        public long PartyListId { get; set; }

        [Required(ErrorMessage = "PartyList ID is null.")]
        public long PositionId { get; set; }
    }
}
