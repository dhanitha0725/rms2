using Application.DTOs.FacilityDtos;
using Application.Features.ManageFacility.AddRooms;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/facilities/{facilityId}/rooms")]
    public class RoomController (IMediator mediator): ControllerBase
    {
        [HttpPost]
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
    }
}
