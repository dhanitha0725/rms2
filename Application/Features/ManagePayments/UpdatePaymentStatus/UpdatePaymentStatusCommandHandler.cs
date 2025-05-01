using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Serilog;

namespace Application.Features.ManagePayments.UpdatePaymentStatus
{
    public class UpdatePaymentStatusCommandHandler(
        IPayhereService payhereService,
        ILogger logger,
        IGenericRepository<Payment, Guid> paymentRepository,
        IGenericRepository<Reservation, int> reservationRepository,
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
                // validate the webhook signature
                var isValid = payhereService.VerifyWebhook(new WebhookNotification(
                    request.MerchantId,
                    request.OrderId,
                    request.PaymentId,
                    request.PayhereAmount,
                    request.PayhereCurrency,
                    request.StatusCode,
                    request.Md5Sig));

                // check if the signature is valid
                if (!isValid)
                {
                    logger.Warning("Invalid Webhook Signature for OrderId: {OrderId}", 
                        request.OrderId);
                    return Result<Guid>.Failure(new Error("Invalid Webhook Signature."));
                }

                // get payments from the database
                var payment = (await paymentRepository.GetAllAsync(cancellationToken))
                    .FirstOrDefault(p => p.OrderID == request.OrderId);

                // check if the payment record exists
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

                // update the payment record
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

                // Get the associated reservation
                var reservation = await reservationRepository.GetByIdAsync(payment.ReservationID, cancellationToken);
                if (reservation == null)
                {
                    logger.Warning("Reservation not found for ReservationID: {ReservationID}",
                        payment.ReservationID);
                    return Result<Guid>.Failure(new Error("Reservation not found."));
                }

                // Update the reservation status based on payment status
                reservation.Status = payment.Status switch
                {
                    "Completed" => ReservationStatus.Confirmed,
                    "Pending" => ReservationStatus.PendingPayment,
                    "Failed" => ReservationStatus.Cancelled,
                    _ => reservation.Status
                };

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
