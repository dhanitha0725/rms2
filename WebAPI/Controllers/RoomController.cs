using Application.DTOs.FacilityDtos;
using Application.Features.ManageFacility.AddRooms;
using Application.Features.ManageFacility.AddRoomTypes;
using Application.Features.ManageFacility.GetRoomTypes;
using Application.Features.ManagePackages.AddRoomPricing;
using Application.Features.ManagePackages.GetRoomPricing;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/facilities/rooms")]
    public class RoomController(IMediator mediator) : ControllerBase
    {
        [HttpPost("add-rooms{facilityId}")]
        public async Task<IActionResult> AddRooms(
            [FromRoute] int facilityId,
            [FromBody] RoomConfigurationDto roomConfigurationDto)
        {
            var command = new AddRoomsCommand(facilityId, roomConfigurationDto);
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.Error.Message });
            }

            return Ok(new
            {
                RoomConfiguration = result.Value
            });
        }

        [HttpPost("add-room-types")]
        public async Task<IActionResult> AddRoomTypes([FromBody] AddRoomTypeCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error.Message });

            return Ok(new { RoomTypeID = result.Value });
        }

        [HttpGet("get-room-types")]
        public async Task<IActionResult> GetRoomTypes()
        {
            var query = new GetRoomTypesQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error.Message });
            return Ok(new { RoomTypes = result.Value });
        }

        [HttpGet("get-room-pricing")]
        public async Task<IActionResult> GetRoomPricing()
        {
            var query = new GetRoomPricingQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error.Message });
            return Ok(new { RoomPricing = result.Value });
        }

        [HttpPost("set-room-pricing")]
        public async Task<IActionResult> SetRoomPricing([FromBody] AddRoomPricingCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error.Message });
            return Ok(new { RoomTypeID = result });
        }
    }
}
