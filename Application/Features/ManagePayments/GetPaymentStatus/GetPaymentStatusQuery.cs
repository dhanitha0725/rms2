using Application.DTOs.Payment;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePayments.GetPaymentStatus
{
    public class GetPaymentStatusQuery : IRequest<Result<PaymentStatusDto>>
    {
        public string OrderId { get; set; }
    }
}