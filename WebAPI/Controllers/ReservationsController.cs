using Application.DTOs.ReservationDtos;
using Application.Features.ManageReservations.AddReservationByCustomer.CompleteReservation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromForm] MakeReservationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values
                    .SelectMany(v => v.Errors
                        .Select(e => e.ErrorMessage)));
            }

            var userId = int.Parse(User.FindFirst("nameid")?.Value ?? "0");
            var userRole = User.FindFirst("role")?.Value ?? "Customer";

            var command = new MakeReservationCommand
            {
                ReservationDto = dto,
                AuthenticatedUserId = userId,
                AuthenticatedUserRole = userRole
            };

            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.Error.Message });
            }

            return Created($"/api/reservations/{result.Value}", new { ReservationId = result.Value });
        }
    }
}
