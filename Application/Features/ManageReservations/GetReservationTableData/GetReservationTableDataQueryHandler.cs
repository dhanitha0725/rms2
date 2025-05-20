using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.GetReservationTableData
{
    public class GetReservationTableDataQueryHandler(
            IReservationRepository reservationRepository,
            ILogger logger)
            : IRequestHandler<GetReservationTableDataQuery, Result<List<ReservationDataDto>>>
    {
        public async Task<Result<List<ReservationDataDto>>> Handle(
            GetReservationTableDataQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Get all reservations with facility information using the repository method
                var reservationDataDtos = await reservationRepository.GetReservationsWithFacilityAsync(cancellationToken);

                if (reservationDataDtos == null || !reservationDataDtos.Any())
                {
                    return Result<List<ReservationDataDto>>.Failure(
                        new Error("No reservations found."));
                }

                return Result<List<ReservationDataDto>>.Success(reservationDataDtos);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while retrieving reservations");
                return Result<List<ReservationDataDto>>.Failure(
                    new Error("An error occurred while retrieving reservations"));
            }
        }
    }
}
