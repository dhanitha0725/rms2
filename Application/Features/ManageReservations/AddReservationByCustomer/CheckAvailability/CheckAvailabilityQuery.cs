using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CheckAvailability
{
    public class CheckAvailabilityQuery : IRequest<Result<bool>>
    {
        public int FacilityID { get; set; }
        public List<SelectedPackageDto>? Packages { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
