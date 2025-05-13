using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IReportRepository
    {
        Task<List<FinancialReport>> GetFinancialReportAsync(
            DateTime startDate, 
            DateTime endDate, 
            int? facilityId, 
            CancellationToken cancellationToken = default);
    }
}
