using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.GetReservationTableData
{
    public class GetReservationTableDataQuery : IRequest<Result<List<ReservationDataDto>>>
    {
    }
}
