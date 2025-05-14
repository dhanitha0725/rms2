using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IReportRepository
    {
        Task<List<FinancialReport>> GetFinancialReportAsync(
            DateTime startDate, 
            DateTime endDate,
            IEnumerable<int>? facilityIds,
            CancellationToken cancellationToken = default);

        Task<List<ReservationReport>> GetReservationReportAsync(
            DateTime startDate,
            DateTime endDate,
            IEnumerable<int>? facilityIds,
            CancellationToken cancellationToken = default);

    }
}
