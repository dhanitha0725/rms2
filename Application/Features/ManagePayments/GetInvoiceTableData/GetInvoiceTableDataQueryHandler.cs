using Application.Abstractions.Interfaces;
using Application.DTOs.InvoiceDtos;
using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Features.ManagePayments.GetInvoiceTableData
{
    public class GetInvoiceTableDataQueryHandler(
            IInvoiceRepository invoiceRepository,
            ILogger logger)
            : IRequestHandler<GetInvoiceTableDataQuery, Result<List<InvoiceTableDataDto>>>
    {
        public async Task<Result<List<InvoiceTableDataDto>>> Handle(
            GetInvoiceTableDataQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var invoiceData = await invoiceRepository.GetInvoiceTableDataAsync(cancellationToken);

                if (invoiceData == null || !invoiceData.Any())
                {
                    logger.Warning("No invoice records found");
                    return Result<List<InvoiceTableDataDto>>.Failure(
                        new Error("No invoice records found"));
                }

                logger.Information("Retrieved {Count} invoice records", invoiceData.Count);
                return Result<List<InvoiceTableDataDto>>.Success(invoiceData);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error retrieving invoice data");
                return Result<List<InvoiceTableDataDto>>.Failure(
                    new Error("An error occurred while retrieving invoice data"));
            }
        }
    }
}
