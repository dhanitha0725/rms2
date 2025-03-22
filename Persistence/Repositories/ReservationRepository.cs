using Application.Abstractions.Interfaces;
using Domain.Entities;

namespace Persistence.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        public Task<Reservation> AddAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
