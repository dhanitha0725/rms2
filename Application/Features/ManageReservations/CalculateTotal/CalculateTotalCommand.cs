using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.CalculateTotal
{
    public class CalculateTotalCommand : IRequest<Result<CalculateTotalResponseDto>>
    {
        public CalculateTotalDto CalculateTotalDto { get; set; }
    }
}
