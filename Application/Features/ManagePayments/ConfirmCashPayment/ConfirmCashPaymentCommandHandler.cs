using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Serilog;

namespace Application.Features.ManagePayments.ConfirmCashPayment;

public class ConfirmCashPaymentCommandHandler(
    IGenericRepository<Payment, Guid> paymentRepository,
    IGenericRepository<Reservation, int> reservationRepository,
    ILogger logger,
    IUnitOfWork unitOfWork)
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

            return Result.Success([]);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            logger.Error(ex, "Error confirming cash payment for PaymentId: {PaymentId}", request.PaymentId);
            return Result.Failure(new Error("An error occurred while confirming the cash payment."));
        }
    }
}

