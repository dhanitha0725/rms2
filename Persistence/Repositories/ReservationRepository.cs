using System.Linq.Expressions;
using Application.Abstractions.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class ReservationRepository(ReservationDbContext context) : IReservationRepository
    {
        public Task<IEnumerable<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation> AddAsync(Reservation entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Reservation entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Reservation, bool>> predicate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation?> AddRangeAsync(IEnumerable<Reservation> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation?> GetByIdWithIncludeAsync(int id, params Expression<Func<Reservation, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation?> GetByIdWithQueryAsync(Func<IQueryable<Reservation>, IQueryable<Reservation>> queryBuilder, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Reservation?> GetReservationDetailsAsync(int reservationId, CancellationToken cancellationToken = default)
        {
            return await context.Reservations
                .Where(r => r.ReservationID == reservationId)
                .Include(r => r.ReservationUserDetail)
                .Include(r => r.Payments)!
                    .ThenInclude(p => p.Documents)
                .Include(r => r.Documents)
                .Include(r => r.ReservedPackages)
                    .ThenInclude(rp => rp.Package)
                        .ThenInclude(p => p.Facility)
                .Include(r => r.ReservedRooms)!
                    .ThenInclude(rr => rr.Room)
                        .ThenInclude(r => r.Facility)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
