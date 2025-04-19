using FluentValidation;

namespace Application.Features.ManageReservations.CreateReservation
{
    public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
    {
        public CreateReservationCommandValidator()
        {
            // validate StartDate and EndDate
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate is required.")
                .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");

            // validate total
            RuleFor(x => x.Total)
                .GreaterThan(0).WithMessage("Total must be greater than 0.");

            // validate customer type
            RuleFor(x => x.CustomerType)
                .NotEmpty().WithMessage("CustomerType is required.")
                .Must(type => type.Equals("private", StringComparison.OrdinalIgnoreCase) || 
                              type.Equals("corporate", StringComparison.OrdinalIgnoreCase) || 
                              type.Equals("public", StringComparison.OrdinalIgnoreCase ))
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
            RuleFor(x => x.UserDetails)
                .NotNull().WithMessage("UserDetails are required.");
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
