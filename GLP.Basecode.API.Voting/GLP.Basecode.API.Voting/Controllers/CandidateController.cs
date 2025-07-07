using GLP.Basecode.API.Voting.Constant;
using GLP.Basecode.API.Voting.Manager;
using GLP.Basecode.API.Voting.Models.ApiModel;
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

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetCandidateBy(long id)
        {
            var retVal = await _canManager.GetCandidateBy(id);
            return retVal.Status switch
            {
                ErrorCode.Success => Ok(new { success = true, data = retVal.Data, message = retVal.SuccessMessage }),
                ErrorCode.NotFound => NotFound(new { success = false, message = retVal.ErrorMessage }),
                _ => StatusCode(500, new { success = false, message = "Unknown error occured." })
            };
        }

        //not tested
        [HttpPost("create")]
        public async Task<IActionResult> CreateCandidate([FromForm] CreateCandidateViewInputModel model)
        {
            var retVal = await _canManager.CreateCandidate(model);

            return retVal.Status switch
            {
                ErrorCode.Success => Ok(new { success = true, message = retVal.SuccessMessage }),
                _ => StatusCode(500, new { success = false, message = "Unknown error occured." })
            };
        }

    }
}
