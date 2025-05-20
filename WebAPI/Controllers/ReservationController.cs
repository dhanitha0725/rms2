using Application.DTOs.ReservationDtos;
using Application.Features.ManageFacility.SelectedFacilityDetails;
using Application.Features.ManageReservations.CalculateTotal;
using Application.Features.ManageReservations.CancelReservation;
using Application.Features.ManageReservations.CheckAvailability;
using Application.Features.ManageReservations.CreateReservation;
using Application.Features.ManageReservations.GetFacilityChartData;
using Application.Features.ManageReservations.GetReservationChartData;
using Application.Features.ManageReservations.GetReservationDetails;
using Application.Features.ManageReservations.GetReservationStats;
using Application.Features.ManageReservations.GetReservationTableData;
using Application.Features.ManageReservations.UpdateReservation;
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

        // Get selected facility details by ID (client side)
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
            return Ok(result);
        }

        // Upload documents (reservation + payment)
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

        // Get reservation table data
        [HttpGet("reservation-data")]
        public async Task<IActionResult> GetReservationTableData()
        {
            var query = new GetReservationTableDataQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        // Get full reservation details by ID
        [HttpGet("reservation-details/{id}")]
        public async Task<IActionResult> GetReservationDetails(int id)
        {
            var query = new GetReservationDetailsQuery { ReservationId = id };
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        [HttpPost("cancel-reservation")]
        public async Task<IActionResult> CancelReservation([FromBody] CancelReservationCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        // update reservation
        [HttpPut("update-reservation")]
        public async Task<IActionResult> UpdateReservation([FromBody] UpdateReservationCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        // Get reservation stats for the last 30 days
        [HttpGet("reservation-stats")]
        public async Task<IActionResult> GetReservationStats()
        {
            var query = new GetReservationStatQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        // Get daily reservation counts
        [HttpGet("daily-reservation-counts")]
        public async Task<IActionResult> GetDailyReservationCounts()
        {
            var query = new GetReservationChartDataQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        // Get facility reservation counts
        [HttpGet("facility-reservation-counts")]
        public async Task<IActionResult> GetFacilityReservationCounts()
        {
            var query = new GetFacilityChartDataQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }
    }
}