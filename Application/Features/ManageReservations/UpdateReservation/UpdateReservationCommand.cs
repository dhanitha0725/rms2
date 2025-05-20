using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.UpdateReservation
{
    public class UpdateReservationCommand : IRequest<Result<ReservationResultDto>>
    {
        public int ReservationId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Total { get; set; }

        // Package and room updates
        public List<BookingItemDto> PackageUpdates { get; set; } = new();
        public List<BookingItemDto> RoomUpdates { get; set; } = new();
    }
}
