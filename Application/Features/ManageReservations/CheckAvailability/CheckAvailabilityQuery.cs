using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.CheckAvailability
{
    public class CheckAvailabilityQuery : IRequest<Result<AvailabilityResponseDto>>
    {
      public CheckAvailabilityDto CheckAvailabilityDto { get; set; }
    }
}
