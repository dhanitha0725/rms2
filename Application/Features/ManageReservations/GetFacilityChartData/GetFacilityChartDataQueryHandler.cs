using System;
using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ManageReservations.GetFacilityChartData
{
    public class GetFacilityChartDataQueryHandler(
        IReservationRepository reservationRepository,
        ILogger logger)
        : IRequestHandler<GetFacilityChartDataQuery, Result<FacilityReservationCountsResponse>>
    {
        public async Task<Result<FacilityReservationCountsResponse>> Handle(
            GetFacilityChartDataQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var results = await reservationRepository.GetFacilityReservationCountsAsync(cancellationToken);

                logger.Information("Successfully retrieved facility reservation counts for the last 30 days");
                return Result<FacilityReservationCountsResponse>.Success(results);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error retrieving facility reservation counts");
                return Result<FacilityReservationCountsResponse>.Failure(
                    new Error("An error occurred while retrieving facility reservation counts"));
            }
        }
    }
}
