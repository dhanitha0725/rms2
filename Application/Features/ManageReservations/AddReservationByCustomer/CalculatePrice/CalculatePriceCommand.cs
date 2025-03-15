using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CalculatePrice
{
    public class CalculatePriceCommand : IRequest<Result<decimal>>
    {
        public string CustomerType { get; set; }
        public List<SelectedPackageDto>? Packages { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
