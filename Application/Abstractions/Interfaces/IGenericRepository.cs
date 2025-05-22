using System.Linq.Expressions;

namespace Application.Abstractions.Interfaces
{
    public interface IGenericRepository<T, Tid> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default);
        
        Task<T?> GetByIdAsync(Tid id, 
            CancellationToken cancellationToken = default);
        
        Task<T> AddAsync(T entity, 
            CancellationToken cancellationToken = default);
        
        Task UpdateAsync(T entity, 
            CancellationToken cancellationToken = default);
        
        Task DeleteAsync(Tid id, 
            CancellationToken cancellationToken = default);
        
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken);
        
        Task<T?> AddRangeAsync (IEnumerable<T> entities, 
            CancellationToken cancellationToken = default);
        
        Task<T?> GetByIdWithIncludeAsync(Tid id, 
            params Expression<Func<T, object>>[] includes);
        
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default);
        
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, 
            CancellationToken cancellationToken = default);
    }
}
    