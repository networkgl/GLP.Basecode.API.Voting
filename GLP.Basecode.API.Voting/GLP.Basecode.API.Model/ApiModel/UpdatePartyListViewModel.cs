using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GLP.Basecode.API.Model.ApiModel
{
    public class UpdatePartyListViewModel
    {
        public string? PartyListName { get; set; } = null!;

        public IFormFile? PartyListImage { get; set; }
    }
}
