using Application.Abstractions.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class PackageRepository(ReservationDbContext context) : IPackageRepository
    {
        public async Task<List<Package>> GetByIdsWithLockAsync(
            IEnumerable<int> packageIds, 
            CancellationToken cancellationToken = default)
        {
            var ids = string.Join(",", packageIds);
            return await context.Packages
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM Packages WITH (UPDLOCK)
                    WHERE PackageID IN ({ids})")
                .Include(p => p.Facility)
                .ThenInclude(f => f.Rooms)
                .ToListAsync(cancellationToken);
        }
    }
}
