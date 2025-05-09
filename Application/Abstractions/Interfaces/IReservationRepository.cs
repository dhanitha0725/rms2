using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IReservationRepository : IGenericRepository<Reservation, int>
    {
        Task<Reservation?> GetReservationDetailsAsync(int reservationId, CancellationToken cancellationToken = default);
    }
}
