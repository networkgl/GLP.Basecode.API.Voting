using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GLP.Basecode.API.BLL.Managers;
using GLP.Basecode.API.BLL.Services;
using GLP.Basecode.API.Model.Enum;
using GLP.Basecode.API.Model.ApiModel;

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
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetPartyListById(long id)
        {
            var retVal = await _partyListManager.GetPartyListById(id);
            return retVal.Status switch
            {
                ErrorCode.Success => Ok(new { success = true, data = retVal.Data, message = retVal.SuccessMessage }),
                ErrorCode.NotFound => NotFound(new { success = false, message = retVal.ErrorMessage }),
                _ => StatusCode(500, new { success = false, message = "Unknown error occured." })
            };
        }

        [HttpGet("get-all-partylist")]
        public async Task<IActionResult> GetAllPartyList()
        {
            var retVal = await _partyListManager.GetAllPartyList();
            return Ok(new { success = true, data = retVal, message = "Party List successfully retrieved." });
        }

        //tested
        [HttpPost("create")]
        public async Task<IActionResult> CreatePartyList([FromForm] CreatePartyListViewInputModel model)
        {
            var retVal = await _partyListManager.CreatePartyList(model);

            if (retVal.Status != ErrorCode.Success)
            {
                return retVal.Status switch
                {
                    ErrorCode.NotFound => NotFound(new { success = false, message = retVal.ErrorMessage }),
                    ErrorCode.Duplicate => Conflict(new { success = false, message = retVal.ErrorMessage }),
                    _ => StatusCode(500, new { success = false, message = "Unknown error occured." })
                };
            }

            return Ok(new { success = true, message = retVal.SuccessMessage });
        }

        //tested
        [HttpPut("update/{id:long}")]
        public async Task<IActionResult> UpdatePartyList(long id, [FromForm] UpdatePartyListViewModel model)
        {
            var retVal = await _partyListManager.UpdatePartyList(id, model);

            if (retVal.Status != ErrorCode.Success)
            {
                return retVal.Status switch
                {
                    ErrorCode.NotFound => NotFound(new { success = false, message = retVal.ErrorMessage }),
                    //ErrorCode.Error => BadRequest(new { success = false, message = retVal.ErrorMessage }),
                    _ => StatusCode(500, new { success = false, message = retVal.ErrorMessage })
                };
            }

            return Ok(new { success = true, message = retVal.SuccessMessage });
        }
    }
}
