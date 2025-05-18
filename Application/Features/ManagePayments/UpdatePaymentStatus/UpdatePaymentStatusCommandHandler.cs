using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application.Features.ManagePayments.UpdatePaymentStatus
{
    public class UpdatePaymentStatusCommandHandler(
        IPayhereService payhereService,
        ILogger logger,
        IGenericRepository<Payment, Guid> paymentRepository,
        IGenericRepository<Reservation, int> reservationRepository,
        IGenericRepository<ReservationUserDetail, int> reservationUserRepository,
        IUnitOfWork unitOfWork,
        IEmailContentService emailContentService,
        IBackgroundTaskQueue backgroundTaskQueue,
        IServiceScopeFactory serviceScopeFactory) :
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
                await reservationRepository.UpdateAsync(reservation, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                // If payment is completed, send confirmation email
                if (payment.Status == "Completed")
                {
                    logger.Information("Email is sending");
                    SchedulePaymentConfirmationEmail(reservation.ReservationID, payment.ReservationUserID);
                    logger.Information("Email sent");
                }

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

        private void SchedulePaymentConfirmationEmail(int reservationId, int reservationUserId)
        {
            // Queue background task to send email
            backgroundTaskQueue.QueueBackgroundWorkItem(async (cancellationToken) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                var userRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<ReservationUserDetail, int>>();
                var emailContentService = scope.ServiceProvider.GetRequiredService<IEmailContentService>();

                logger.Information("Sending payment confirmation email for reservation {ReservationId}.", reservationId);

                try
                {
                    // Get user email
                    var user = await userRepository.GetByIdAsync(reservationUserId, cancellationToken);
                    if (user == null || string.IsNullOrEmpty(user.Email))
                    {
                        logger.Warning("User not found or email missing for ReservationUserID: {UserId}", reservationUserId);
                        return;
                    }

                    // Generate and send email
                    var emailBody = await emailContentService.GeneratePaymentConfirmationEmailBodyAsync(reservationId, cancellationToken);
                    await emailService.SendEmailAsync(user.Email, "Payment Confirmation", emailBody);
                    logger.Information("Payment confirmation email sent for reservation {ReservationId}.", reservationId);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error sending payment confirmation email for reservation {ReservationId}.", reservationId);
                }
            });

            logger.Information("Scheduled payment confirmation email for reservation {ReservationId}.", reservationId);
        }
    }
}
