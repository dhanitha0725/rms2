using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.CheckAvailability
{
    public class CheckAvailabilityQuery : IRequest<Result<AvailabilityResultDto>>
    {
        public int FacilityId { get; set; }
        public List<RoomRequest> Rooms { get; set; } = new List<RoomRequest>();
        public List<PackageRequest> Packages { get; set; } = new List<PackageRequest>();
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class RoomRequest
    {
        public string RoomType { get; set; }
        public int Quantity { get; set; }
    }

    public class PackageRequest
    {
        public int PackageId { get; set; }
    }
}
