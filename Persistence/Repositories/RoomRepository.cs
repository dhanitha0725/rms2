using System.Linq.Expressions;
using Application.Abstractions.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class RoomRepository (ReservationDbContext context) : IRoomRepository
    {
        public Task<IEnumerable<Room>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Room?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Room> AddAsync(Room entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Room entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Room, bool>> predicate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Room?> AddRangeAsync(IEnumerable<Room> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Room?> GetByIdWithIncludeAsync(int id, params Expression<Func<Room, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<Room?> GetByIdWithQueryAsync(Func<IQueryable<Room>, IQueryable<Room>> queryBuilder, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Room> GetQuery()
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<Room, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<Room, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        // This method retrieves room pricing information for a
        // specific facility and a list of room type IDs.
        public async Task<List<RoomPricing>> GetRoomPricingWithRoomTypeAsync(
            int facilityId, List<int> roomTypeIds, 
            CancellationToken cancellationToken)
        {
            return await context.RoomPricings
                .Include(rp => rp.RoomType)
                .Where(rp => rp.FacilityID == facilityId && roomTypeIds.Contains(rp.RoomTypeID))
                .ToListAsync(cancellationToken);
        }
    }
}
