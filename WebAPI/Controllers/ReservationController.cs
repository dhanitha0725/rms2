using Application.DTOs.ReservationDtos;
using Application.Features.ManageFacility.SelectedFacilityDetails;
using Application.Features.ManagePayments.CreatePayment;
using Application.Features.ManageReservations.CalculateTotal;
using Application.Features.ManageReservations.CheckAvailability;
using Application.Features.ManageReservations.CreateReservation;
using Application.Features.ManageReservations.UploadDocument;
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

        [HttpPost("createReservation")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            //check customer type
            if (command.CustomerType.ToLower() == "private")
            {
                // private customers --> direct payment
                var paymentCommand = new CreatePaymentCommand
                {
                    OrderId = result.Value.OrderId,
                    Amount = result.Value.Total,
                    Currency = "LKR",
                    FirstName = command.UserDetails.FirstName,
                    LastName = command.UserDetails.LastName,
                    Email = command.UserDetails.Email,
                    Phone = command.UserDetails.Phone,
                    Address = command.UserDetails.Address,
                    City = command.UserDetails.City,
                    Country = command.UserDetails.Country,
                    Items = command.Items.ToString() ?? throw new InvalidOperationException(),
                    ReservationId = result.Value.ReservationId
                };

                var paymentResult = await mediator.Send(paymentCommand);

                if (!paymentResult.IsSuccess)
                {
                    return BadRequest(paymentResult.Error);
                }

                // Return the payment initiation response
                return Ok(new
                {
                    result.Value.ReservationId,
                    paymentUrl = paymentResult.Value.ActionUrl,
                });
            }

            // public or corporate customers --> no direct payment
            return Ok(new
            {
                result.Value.ReservationId,
                Message = "Reservation created successfully. Pending Approval.",
            });
        }

        [HttpPost("uploadDocument")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentsCommand command)
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
