using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IPackageRepository
    {
        Task<List<Package>> GetByIdsWithLockAsync (
            IEnumerable<int> packageIds, 
            CancellationToken cancellationToken = default);
    }
}
