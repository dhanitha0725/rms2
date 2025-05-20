using FluentValidation;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Application.Features.ManageReservations.CreateReservation
{
    public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
    {
        public CreateReservationCommandValidator()
        {
            // validate StartDate and EndDate conditionally
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            // Only enforce EndDate > StartDate when there are room bookings
            // For package-only bookings, allow same-day reservations (EndDate can equal StartDate)
            RuleFor(x => x)
                .Custom((reservation, context) =>
                {
                    bool hasRooms = reservation.Items.Any(i => i.Type.Equals("room", StringComparison.OrdinalIgnoreCase));

                    // Always require EndDate
                    if (reservation.EndDate == default)
                    {
                        context.AddFailure("EndDate", "EndDate is required.");
                        return;
                    }

                    // For rooms, enforce strict EndDate > StartDate
                    if (hasRooms && reservation.EndDate <= reservation.StartDate)
                    {
                        context.AddFailure("EndDate", "EndDate must be after StartDate for room reservations.");
                    }
                    // For packages-only, allow same-day (EndDate can be equal to StartDate)
                    else if (!hasRooms && reservation.EndDate < reservation.StartDate)
                    {
                        context.AddFailure("EndDate", "EndDate cannot be before StartDate.");
                    }
                });

            RuleFor(x => x.UserDetails)
                .NotNull().WithMessage("User details are required.");

            // validate total
            RuleFor(x => x.Total)
                .GreaterThan(0).WithMessage("Total must be greater than 0.");

            // validate customer type
            RuleFor(x => x.CustomerType)
                .NotEmpty().WithMessage("CustomerType is required.")
                .Must(type => type.Equals("private", StringComparison.OrdinalIgnoreCase) ||
                              type.Equals("corporate", StringComparison.OrdinalIgnoreCase) ||
                              type.Equals("public", StringComparison.OrdinalIgnoreCase))
                .WithMessage("CustomerType must be either 'public', 'corporate' or 'private'.");

            // validate items
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one item is required.");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ItemId)
                    .GreaterThan(0).WithMessage("ItemId must be greater than 0.");
                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
                items.RuleFor(i => i.Type)
                    .NotEmpty().WithMessage("Type is required.")
                    .Must(type => type.Equals("package", StringComparison.OrdinalIgnoreCase) ||
                                  type.Equals("room", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Type must be either 'package' or 'room'.");
            });

            // validate user details
            RuleFor(x => x.UserDetails.FirstName)
                .NotEmpty().WithMessage("FirstName is required.");
            RuleFor(x => x.UserDetails.LastName)
                .NotEmpty().WithMessage("LastName is required.");
            RuleFor(x => x.UserDetails.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
