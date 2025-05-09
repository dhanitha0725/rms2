using Application.DTOs.FacilityDtos;
using Application.Features.ManageFacility.AddRooms;
using Application.Features.ManageFacility.AddRoomTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/facilities/rooms")]
    public class RoomController (IMediator mediator): ControllerBase
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
    }
}
