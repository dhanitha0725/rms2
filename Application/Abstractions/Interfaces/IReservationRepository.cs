using Domain.Entities;
using Application.DTOs.ReservationDtos;

namespace Application.Abstractions.Interfaces
{
    public interface IReservationRepository : IGenericRepository<Reservation, int>
    {
        Task<Reservation?> GetReservationDetailsAsync(int reservationId, CancellationToken cancellationToken = default);

        Task<List<ReservationDataDto>> GetReservationsWithFacilityAsync(CancellationToken cancellationToken = default);

        Task<ReservationStatsDto> GetReservationStatsForLast30DaysAsync(CancellationToken cancellationToken = default);
    }
}
