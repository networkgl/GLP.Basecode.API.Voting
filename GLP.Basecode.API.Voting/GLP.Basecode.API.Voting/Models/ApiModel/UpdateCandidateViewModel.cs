namespace GLP.Basecode.API.Voting.Models.ApiModel
{
    public class UpdateCandidateViewModel
    {
        public IFormFile? CandidateImage { get; set; }
        public long? NewPositionId { get; set; }
    }
}
