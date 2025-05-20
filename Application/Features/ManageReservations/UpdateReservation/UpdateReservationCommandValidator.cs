using Application.Abstractions.Interfaces;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;

namespace Application.Features.ManageReservations.UpdateReservation
{
    public class UpdateReservationCommandValidator : AbstractValidator<UpdateReservationCommand>
    {
        public UpdateReservationCommandValidator(
            IGenericRepository<Reservation, int> reservationRepository)
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty().WithMessage("Reservation ID is required")
                .GreaterThan(0).WithMessage("Reservation ID must be greater than 0");

            // Validate that the reservation exists
            RuleFor(x => x.ReservationId)
                .MustAsync(async (id, cancellation) =>
                {
                    var reservation = await reservationRepository.GetByIdAsync(id, cancellation);
                    return reservation != null;
                })
                .WithMessage(x => $"Reservation with ID {x.ReservationId} not found");

            // Validate that the reservation is not in a terminal state
            RuleFor(x => x.ReservationId)
                .MustAsync(async (id, cancellation) =>
                {
                    var reservation = await reservationRepository.GetByIdAsync(id, cancellation);
                    return reservation != null &&
                           reservation.Status != ReservationStatus.Cancelled &&
                           reservation.Status != ReservationStatus.Completed &&
                           reservation.Status != ReservationStatus.Expired;
                })
                .WithMessage("Cannot update reservation in Cancelled, Completed, or Expired status");

            // Date validation when dates are provided
            When(x => x.StartDate.HasValue && x.EndDate.HasValue, () =>
            {
                RuleFor(x => x.StartDate)
                    .NotNull().WithMessage("Start date cannot be null");

                RuleFor(x => x.EndDate)
                    .NotNull().WithMessage("End date cannot be null")
                    .GreaterThan(x => x.StartDate)
                    .WithMessage("End date must be after start date");

                RuleFor(x => x.StartDate)
                    .GreaterThan(DateTime.Today)
                    .WithMessage("Start date must be in the future");
            });

            // Validate package updates if any
            RuleForEach(x => x.PackageUpdates)
                .ChildRules(package =>
                {
                    package.RuleFor(p => p.ItemId)
                        .GreaterThan(0).WithMessage("Package ID must be greater than 0");
                    package.RuleFor(p => p.Type)
                        .Equal("package").WithMessage("Type must be 'package'");
                });

            // Validate room updates if any
            RuleForEach(x => x.RoomUpdates)
                .ChildRules(room =>
                {
                    room.RuleFor(r => r.ItemId)
                        .GreaterThan(0).WithMessage("Room type ID must be greater than 0");
                    room.RuleFor(r => r.Quantity)
                        .GreaterThan(0).WithMessage("Room quantity must be greater than 0");
                    room.RuleFor(r => r.Type)
                        .Equal("room").WithMessage("Type must be 'room'");
                });

            // Total validation if provided
            When(x => x.Total.HasValue, () =>
            {
                RuleFor(x => x.Total)
                    .GreaterThan(0).WithMessage("Total must be greater than 0");
            });
        }
    }
}