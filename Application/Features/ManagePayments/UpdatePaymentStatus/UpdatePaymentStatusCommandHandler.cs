using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManagePayments.UpdatePaymentStatus
{
    public class UpdatePaymentStatusCommandHandler(
        IPayhereService payhereService,
        ILogger logger,
        IGenericRepository<Payment, Guid> paymentRepository,
        IUnitOfWork unitOfWork) :
        IRequestHandler<UpdatePaymentStatusCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(
            UpdatePaymentStatusCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var isValid = payhereService.VerifyWebhook(new WebhookNotification(
                    request.MerchantId,
                    request.OrderId,
                    request.PaymentId,
                    request.PayhereAmount,
                    request.PayhereCurrency,
                    request.StatusCode,
                    request.Md5Sig));

                if (!isValid)
                {
                    logger.Warning("Invalid Webhook Signature for OrderId: {OrderId}", 
                        request.OrderId);
                    return Result<Guid>.Failure(new Error("Invalid Webhook Signature."));
                }

                // get payments from the database
                var payment = (await paymentRepository.GetAllAsync(cancellationToken))
                    .FirstOrDefault(p => p.OrderID == request.OrderId);

                if (payment == null)
                {
                    logger.Warning("Payment record not found for OrderId: {OrderId}", 
                        request.OrderId);
                    return Result<Guid>.Failure(new Error("Payment record not found."));
                }

                // change the status according to the status code from payhere
                payment.Status = request.StatusCode switch
                {
                    2 => "Completed",
                    0 => "Pending",
                    _ => "Failed"
                };

                payment.GatewayTransactionID = request.PaymentId;
                if (decimal.TryParse(request.PayhereAmount, out var amountPaid))
                {
                    payment.AmountPaid = amountPaid;
                }
                else
                {
                    logger.Warning("Invalid PayhereAmount format for OrderId: {OrderId}",
                        request.OrderId);
                    return Result<Guid>.Failure(new Error("Invalid PayhereAmount format."));
                }

                await paymentRepository.UpdateAsync(payment, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("Payment Status Updated: {PaymentId}, Status: {Status}",
                    request.PaymentId, payment.Status);
                return Result<Guid>.Success(payment.PaymentID);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error("Payment Status Update Failed: {Message}", e.Message);
                throw;
            }
        }
    }
}
