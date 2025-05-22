using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.GetReservationChartData
{
    public class GetReservationChartDataQuery : IRequest<Result<DailyReservationCountsResponse>>
    {
    }

}
