using Application.DTOs.InvoiceDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePayments.GetInvoiceTableData
{
    public class GetInvoiceTableDataQuery : IRequest<Result<List<InvoiceTableDataDto>>>
    {
    }
}
