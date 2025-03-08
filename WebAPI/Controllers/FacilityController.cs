using Application.DTOs.FacilityDtos;
using Application.Features.ManageFacility.AddFacility.Commands;
using Application.Features.ManageFacility.AddFacilityType;
using Application.Features.ManageFacility.GetFacilityDetails;
using Application.Features.ManageFacility.GetFacilityNames;
using Application.Features.ManageFacility.GetFacilityTypes;
using Application.Features.ManageFacility.UpdateFacility;
using Application.Features.ManageFacility.UploadImages;
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
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddFacility([FromBody] AddFacilityDto addFacilityDto)
        {
            var command = new AddFacilityCommand(addFacilityDto);
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.Error.Message });
            }

            return Ok(new
            {
                Facility = result.Value
            });
        }

        [HttpPost("new-facility-type")]
        public async Task<IActionResult> AddNewFacilityType([FromBody] AddFacilityTypeDto addFacilityTypeDto)
        {
            var command = new AddFacilityTypeCommand(addFacilityTypeDto);
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.Error.Message });
            }

            return Ok(new { FacilityTypeId = result.Value });
        }

        [HttpPost("{facilityId}/images")]
        public async Task<IActionResult> AddFacilityImages(
            [FromRoute] int facilityId,
            [FromForm] AddFacilityImagesDto dto)
        {
            var command = new AddFacilityImagesCommand(facilityId, dto);
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.Error.Message });
            }

            return Ok(new { ImageUrls = result.Value });
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFacility([FromRoute] int id,
            [FromBody] UpdateFacilityDto updateFacilityDto, string facilityId)
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
            var query = new GetFacilityTypeQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.Error.Message });
            }

            return Ok(result.Value);
        }

        [HttpGet("select-facility-list")]
        public async Task<IActionResult> GetFacilitySelectList()
        {
            var query = new GetFacilityNamesQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetFacilityDetails()
        {
            var query = new GetFacilityDetailsQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
