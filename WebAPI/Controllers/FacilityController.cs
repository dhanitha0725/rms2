using Application.DTOs;
using Application.Features.AddFacility.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class FacilityController : ControllerBase
    {
       private readonly IMediator _mediator;

        public FacilityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddFacility([FromBody] AddFacilityDto addFacilityDto)
        {
            var command = new AddFacilityCommand { FacilityDto = addFacilityDto };
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                //return BadRequest(result);
                return BadRequest(new { Error = result.Error.Message });
            }

           return Ok(new { FacilityId = result.Value });
        }
    }
}
