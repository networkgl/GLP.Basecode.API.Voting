using Microsoft.AspNetCore.Http;

namespace GLP.Basecode.API.Model.ApiModel
{ 
    public class UpdateCandidateViewModel
    {
        public IFormFile? CandidateImage { get; set; }
        public long? NewPositionId { get; set; }
    }
}
