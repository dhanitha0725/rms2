using Application.DTOs.ReservationDtos;
using Application.Features.ManageFacility.SelectedFacilityDetails;
using Application.Features.ManageReservations.CalculateTotal;
using Application.Features.ManageReservations.CheckAvailability;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController(IMediator mediator) : ControllerBase
    {
        [HttpPost("checkAvailability")]
        public async Task<IActionResult> CheckAvailability([FromBody] CheckAvailabilityDto dto)
        {
            var query = new CheckAvailabilityQuery { CheckAvailabilityDto = dto };
            var result = await mediator.Send(query);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSelectedFacilityDetails(int id)
        {
            var query = new GetSelectedFacilityDetailsQuery { FacilityId = id };
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result);
        }

        [HttpPost("calculateTotal")]
        public async Task<IActionResult> CalculateTotal([FromBody] CalculateTotalCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result);
        }
    }
}
