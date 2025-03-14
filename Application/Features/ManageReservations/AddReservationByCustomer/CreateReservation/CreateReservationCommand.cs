using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CreateReservation
{
    public class CreateReservationCommand : IRequest<Result<int>>
    {
        public int UserId { get; set; }
        public int FacilityId { get; set; }
        public List<SelectedPackageDto> Packages { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Total { get; set; }
        public string CustomerType { get; set; }
        public string CreatedBy { get; set; }
    }
}
