using FluentValidation;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CreateGuestUser
{
    public class CreateGuestUserCommandValidator : AbstractValidator<CreateGuestUserCommand>
    {
        public CreateGuestUserCommandValidator()
        {
            RuleFor(x => x.GuestFirstName)
                .NotEmpty().WithMessage("First Name is required")
                .MaximumLength(50).WithMessage("First Name must not exceed 50 characters");
            RuleFor(x => x.GuestLastName)
                .NotEmpty().WithMessage("Last Name is required")
                .MaximumLength(50).WithMessage("Last Name must not exceed 50 characters");
            RuleFor(x => x.GuestEmail)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");
            RuleFor(x => x.GuestPhone)
                .NotEmpty().WithMessage("Phone Number is required")
                .Matches(@"^\d{10}$").WithMessage("Phone Number is not valid");
        }
    }
}
