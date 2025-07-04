using System.ComponentModel.DataAnnotations;

namespace GLP.Basecode.API.Voting.Models.CustomModel
{
    public class UpdatePartyListViewModel
    {
        public string? PartyListName { get; set; } = null!;

        public IFormFile? PartyListImage { get; set; }
    }
}
