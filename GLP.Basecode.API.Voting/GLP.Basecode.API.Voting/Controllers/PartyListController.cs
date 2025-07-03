using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GLP.Basecode.API.Voting.Models.CustomModel;
using GLP.Basecode.API.Voting.Manager;
using GLP.Basecode.API.Voting.Constant;

namespace GLP.Basecode.API.Voting.Controllers
{
    [Authorize(Roles = "SBO Admin")]
    [ApiController]
    [Route("api/partylist")]
    public class PartyListController : ControllerBase
    {
        private readonly PartyListManager _partyListManager;
        public PartyListController(PartyListManager partListManager)
        {
            _partyListManager = partListManager;
        }

        //tested
        [HttpPost("create/party-list")]
        public async Task<IActionResult> AddPartyList([FromForm] CreatePartyListViewInputModel model)
        {
            var retVal = await _partyListManager.CreatePartyList(model);

            if (retVal.Status != ErrorCode.Success)
            {
                return retVal.Status switch
                {
                    ErrorCode.Error => NotFound(new { success = false, message = retVal.ErrorMessage }),
                    ErrorCode.Duplicate => Conflict(new { success = false, message = retVal.ErrorMessage }),
                    _ => StatusCode(500, new { success = false, message = "Unknown error occurred." })
                };
            }

            return Ok(new { success = true, message = retVal.SuccessMessage });
        }

        //tested
        [HttpPost("{id:long}")]
        public async Task<IActionResult> GetPartyListById(long id)
        {
            var result = await _partyListManager.GetPartyListById(id);

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Party List not found."
                });
            }

            return Ok(new
            {
                success = true,
                data = result,
                message = "Party List successfully retrieved."
            });
        }
    }
}
