using System.Linq.Expressions;
using Application.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class GenericRepository<T, Tid>(
        ReservationDbContext context) 
        : IGenericRepository<T, Tid>
        where T : class
    {
        public async Task<T> AddAsync(
            T entity, 
            CancellationToken cancellationToken = default)
        {
            await context.Set<T>().AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task DeleteAsync(
            Tid id, 
            CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken)
        {
            return await context.Set<T>().AnyAsync(predicate, cancellationToken);
        }

        public async Task<T?> AddRangeAsync(
            IEnumerable<T> entities, 
            CancellationToken cancellationToken = default)
        {
            await context.Set<T>().AddRangeAsync(entities, cancellationToken);
            return entities.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await context.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(
            Tid id, 
            CancellationToken cancellationToken = default)
        {
            return await context.Set<T>().FindAsync([id], cancellationToken);
        }

        public Task UpdateAsync(
            T entity, 
            CancellationToken cancellationToken = default)
        {
            context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public IQueryable<T> LockForUpdate(IQueryable<T> query)
        {
            return query.TagWith("FOR UPDATE");
        }
    }
}
