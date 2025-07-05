using GLP.Basecode.API.Voting.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLP.Basecode.API.Voting.Controllers
{
    [Authorize(Roles = "SBO Admin")]
    [ApiController]
    [Route("api/candidate")]
    public class CandidateController : ControllerBase
    {
        private readonly CandidateManager _canManager;
        public CandidateController(CandidateManager candidateManager)
        {
            _canManager = candidateManager;
        }

        [HttpGet("{id:long}/{posName}")]
        public IActionResult GetCandidateBy(long id, string posName)
        {
            return Ok();
        }

    }
}
