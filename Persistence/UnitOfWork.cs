using Application.Abstractions.Interfaces;
using Persistence.DbContexts;

namespace Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ReservationDbContext _context;

        public UnitOfWork(ReservationDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
