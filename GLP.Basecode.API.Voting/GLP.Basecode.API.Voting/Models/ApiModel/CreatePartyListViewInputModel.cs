using System.ComponentModel.DataAnnotations;

namespace GLP.Basecode.API.Voting.Models.CustomModel
{
    public class CreatePartyListViewInputModel
    {
        [Required (ErrorMessage = "Party List name cannot be empty.")]
        public string PartyListName { get; set; } = null!;

        [Required (ErrorMessage = "Please add Party List image before saving.")]
        public IFormFile PartyListImage { get; set; } = null!;

    }
}
