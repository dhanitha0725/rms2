using Application.DTOs;
using Application.Features.ManageFacility.AddFacility.Commands;
using Application.Features.ManageFacility.UpdateFacility;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class FacilityController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddFacility([FromBody] AddFacilityDto addFacilityDto)
        {
            var command = new AddFacilityCommand { FacilityDto = addFacilityDto };
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                //return BadRequest(result);
                return BadRequest(new { Error = result.Error.Message });
            }

            return Ok(new { FacilityId = result.Value });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFacility(
        [FromRoute]int id, 
        [FromBody] UpdateFacilityDto updateFacilityDto)
        {
            var command = new UpdateFacilityCommand
            {
                FacilityId = id, UpdateFacilityDto = updateFacilityDto
            };
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.Error.Message });
            }
            return Ok(new { FacilityId = result.Value });
        }
}
}
