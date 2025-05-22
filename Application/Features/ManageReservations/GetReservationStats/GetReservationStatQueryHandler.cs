using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.GetReservationStats
{
    public class GetReservationStatQueryHandler(
            IReservationRepository reservationRepository,
            ILogger logger)
            : IRequestHandler<GetReservationStatQuery, Result<ReservationStatsDto>>
    {
        public async Task<Result<ReservationStatsDto>> Handle(
            GetReservationStatQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var stats = await reservationRepository.GetReservationStatsForLast30DaysAsync(cancellationToken);

                return Result<ReservationStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error retrieving reservation statistics");
                return Result<ReservationStatsDto>.Failure(
                    new Error("An error occurred while retrieving reservation statistics"));
            }
        }
    }
}
