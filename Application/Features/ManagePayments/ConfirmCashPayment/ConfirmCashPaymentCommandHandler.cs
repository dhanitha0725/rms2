using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application.Features.ManagePayments.ConfirmCashPayment;

public class ConfirmCashPaymentCommandHandler(
    IGenericRepository<Payment, Guid> paymentRepository,
    IGenericRepository<Reservation, int> reservationRepository,
    ILogger logger,
    IUnitOfWork unitOfWork,
    IBackgroundTaskQueue backgroundTaskQueue,
    IServiceScopeFactory serviceScopeFactory)
    : IRequestHandler<ConfirmCashPaymentCommand, Result>
{
    public async Task<Result> Handle(
        ConfirmCashPaymentCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // get the payment
            var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
            {
                logger.Warning("Payment not found for PaymentId: {PaymentId}", request.PaymentId);
                return Result.Failure(new Error("Payment not found."));
            }

            // Validate payment method and status
            if (payment.Method != nameof(PaymentMethods.Cash))
            {
                logger.Warning("Payment {PaymentId} is not a cash payment (Method: {Method}).",
                payment.PaymentID, payment.Method);
                return Result.Failure(new Error($"Payment with ID {request.PaymentId} is not a cash payment."));
            }

            // validate amount
            if (request.AmountPaid <= 0)
            {
                logger.Warning("Invalid AmountPaid {AmountPaid} for Payment {PaymentId}.",
                    request.AmountPaid, payment.PaymentID);
                return Result.Failure(new Error("AmountPaid must be greater than zero."));
            }

            // Retrieve the reservation
            var reservation = await reservationRepository.GetByIdAsync(payment.ReservationID, cancellationToken);
            if (reservation == null)
            {
                logger.Warning("Reservation {ReservationId} not found for Payment {PaymentId}.",
                    payment.ReservationID, payment.PaymentID);
                return Result.Failure(new Error($"Reservation with ID {payment.ReservationID} not found."));
            }

            // Update payment
            payment.AmountPaid = request.AmountPaid;
            payment.Status = "Completed";
            await paymentRepository.UpdateAsync(payment, cancellationToken);
            logger.Information("Payment {PaymentId} status updated to Completed with AmountPaid {AmountPaid}.",
                payment.PaymentID, request.AmountPaid);

            // Update reservation
            reservation.Status = ReservationStatus.Confirmed;
            reservation.UpdatedDate = DateTime.UtcNow;
            await reservationRepository.UpdateAsync(reservation, cancellationToken);
            logger.Information("Reservation {ReservationId} status updated to Confirmed after cash payment confirmation.", reservation.ReservationID);


            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            // Schedule confirmation email
            SchedulePaymentConfirmationEmail(reservation.ReservationID, payment.ReservationUserID);
            logger.Information("Payment confirmation email scheduled for reservation {ReservationId}.", reservation.ReservationID);

            return Result.Success([]);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            logger.Error(ex, "Error confirming cash payment for PaymentId: {PaymentId}", request.PaymentId);
            return Result.Failure(new Error("An error occurred while confirming the cash payment."));
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

