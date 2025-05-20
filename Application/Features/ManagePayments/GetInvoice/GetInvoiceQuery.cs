using System;
using Application.DTOs.InvoiceDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePayments.GetInvoice
{
    public class GetInvoiceQuery : IRequest<Result<InvoiceDetailsDto>>
    {
        public int InvoiceId { get; set; }
    }
}
