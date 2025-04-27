using Domain.Common;
using MediatR;

namespace Application.Features.ManagePayments.UpdatePaymentStatus
{
    public sealed record UpdatePaymentStatusCommand(
        string MerchantId,
        string OrderId,
        string PaymentId,
        string PayhereAmount,
        string PayhereCurrency,
        int StatusCode,
        string Md5Sig) : IRequest<Result<Guid>>
    {
    }
}
