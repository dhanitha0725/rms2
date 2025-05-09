using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageReservations.GetReservationTableData
{
    public class GetReservationTableDataQueryHandler(
            IGenericRepository<Reservation, int> reservationRepository)
            : IRequestHandler<GetReservationTableDataQuery, Result<List<ReservationDataDto>>>
    {
        public async Task<Result<List<ReservationDataDto>>> Handle(
            GetReservationTableDataQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Get all reservations
                var reservations = await reservationRepository.GetAllAsync(cancellationToken);

                if (!reservations.Any())
                {
                    return Result<List<ReservationDataDto>>.Failure(
                        new Error("No reservations found."));
                }

                // Map to DTOs
                var reservationDataDtos = reservations.Select(r => new ReservationDataDto
                {
                    ReservationId = r.ReservationID,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    CreatedDate = r.CreatedDate,
                    Total = r.Total,
                    Status = r.Status.ToString(),
                    UserType = r.UserType
                }).ToList();

                return Result<List<ReservationDataDto>>.Success(reservationDataDtos);
            }
            catch (Exception ex)
            {
                return Result<List<ReservationDataDto>>.Failure(
                    new Error($"An error occurred while retrieving reservations: {ex.Message}"));
            }
        }
    }
   
}
