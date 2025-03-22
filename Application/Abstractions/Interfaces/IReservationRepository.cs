using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation> AddAsync(int id, CancellationToken cancellationToken = default);
    }
}
