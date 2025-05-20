using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.GetReservationChartData
{
    public class GetReservationChartDataQueryHandler(
        IReservationRepository reservationRepository,
        ILogger logger)
        : IRequestHandler<GetReservationChartDataQuery, Result<DailyReservationCountsResponse>>
    {
        public async Task<Result<DailyReservationCountsResponse>> Handle(
            GetReservationChartDataQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var results = await reservationRepository.GetDailyReservationCountsAsync(
                    cancellationToken: cancellationToken);

                logger.Information("Successfully retrieved daily reservation counts");
                return Result<DailyReservationCountsResponse>.Success(results);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error retrieving daily reservation counts");
                return Result<DailyReservationCountsResponse>.Failure(
                    new Error("An error occurred while retrieving daily reservation counts"));
            }
        }
    }
}
