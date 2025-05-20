using Application.Abstractions.Interfaces;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;

namespace Application.Features.ManageReservations.CancelReservation
{
    public class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
    {
        public CancelReservationCommandValidator(IGenericRepository<Reservation, int> reservationRepository)
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty().WithMessage("Reservation ID is required.")
                .GreaterThan(0).WithMessage("Reservation ID must be greater than 0.");

            // Check if reservation exists
            RuleFor(x => x.ReservationId)
                .MustAsync(async (id, cancellationToken) =>
                    await reservationRepository.ExistsAsync(r => r.ReservationID == id, cancellationToken))
                .WithMessage(x => $"Reservation with ID {x.ReservationId} does not exist.");

            // Check if reservation is not already cancelled, expired, or completed
            RuleFor(x => x.ReservationId)
                .MustAsync(async (id, cancellationToken) =>
                {
                    var reservation = await reservationRepository.GetByIdAsync(id, cancellationToken);
                    return reservation != null
                        && reservation.Status != ReservationStatus.Cancelled
                        && reservation.Status != ReservationStatus.Expired
                        && reservation.Status != ReservationStatus.Completed;
                })
                .WithMessage("Reservation is already cancelled, completed, or expired.");

            // Ensure reservation is in a valid state for cancellation
            RuleFor(x => x.ReservationId)
                .MustAsync(async (id, cancellationToken) =>
                {
                    var reservation = await reservationRepository.GetByIdAsync(id, cancellationToken);
                    if (reservation == null) return false;

                    // Define which statuses can be cancelled
                    var cancellableStatuses = new[] {
                        ReservationStatus.PendingApproval,
                        ReservationStatus.PendingPayment,
                        ReservationStatus.PendingPaymentVerification,
                        ReservationStatus.PendingCashPayment,
                        ReservationStatus.Confirmed
                    };

                    return Array.Exists(cancellableStatuses, status => status == reservation.Status);
                })
                .WithMessage("Reservation cannot be cancelled in its current state.");
        }
    }
}