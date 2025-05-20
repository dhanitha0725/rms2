using Application.DTOs.InvoiceDtos;
using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<Invoice, int>
    {
        Task<List<InvoiceTableDataDto>> GetInvoiceTableDataAsync(CancellationToken cancellationToken = default);

        Task<InvoiceDetailsDto> GetInvoiceDetailsAsync(int invoiceId, CancellationToken cancellationToken = default);
    }
}