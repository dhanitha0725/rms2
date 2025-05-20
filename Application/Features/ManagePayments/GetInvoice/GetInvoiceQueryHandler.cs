using Application.Abstractions.Interfaces;
using Application.DTOs.InvoiceDtos;
using Domain.Common;
using MediatR;
using Serilog;

namespace Application.Features.ManagePayments.GetInvoice
{
    public class GetInvoiceQueryHandler(
            IInvoiceRepository invoiceRepository,
            ILogger logger)
            : IRequestHandler<GetInvoiceQuery, Result<InvoiceDetailsDto>>
    {
        public async Task<Result<InvoiceDetailsDto>> Handle(
            GetInvoiceQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var invoiceDetails = await invoiceRepository.GetInvoiceDetailsAsync(
                    request.InvoiceId, cancellationToken);

                if (invoiceDetails == null)
                {
                    logger.Warning("Invoice with ID {InvoiceId} not found", request.InvoiceId);
                    return Result<InvoiceDetailsDto>.Failure(
                        new Error($"Invoice with ID {request.InvoiceId} not found"));
                }

                logger.Information("Retrieved details for invoice {InvoiceId}", request.InvoiceId);
                return Result<InvoiceDetailsDto>.Success(invoiceDetails);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error retrieving invoice details for invoice {InvoiceId}", request.InvoiceId);
                return Result<InvoiceDetailsDto>.Failure(
                    new Error("An error occurred while retrieving invoice details"));
            }
        }
    }
}
