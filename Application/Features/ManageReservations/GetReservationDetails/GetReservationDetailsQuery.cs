using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.GetReservationDetails
{
    public class GetReservationDetailsQuery : IRequest<Result<ReservationDetailsDto>>
    {
        public int ReservationId { get; set; }
    }
}
