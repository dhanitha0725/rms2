using Application.DTOs.FacilityDtos;
using Application.Features.ManageFacility.AddFacility.Commands;
using Application.Features.ManageFacility.GetFacilityTypes;
using Application.Features.ManageFacility.UpdateFacility;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacilityController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        //[Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> UpdateFacility([FromRoute]int id, [FromBody] UpdateFacilityDto updateFacilityDto)
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

        [HttpGet("facility-types")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFacilityTypes()
        {
            var query = new FacilityTypeQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.Error.Message });
            }
            return Ok(result.Value);
        }
    }
}
