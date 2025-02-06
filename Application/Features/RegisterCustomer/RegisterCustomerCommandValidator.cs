using FluentValidation;

namespace Application.Features.RegisterCustomer
{
    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
    {
        public RegisterCustomerCommandValidator()
        {
            RuleFor(x => x.RegisterCustomerDto.FirstName)
                .NotEmpty().WithMessage("First Name is required");

            RuleFor(x => x.RegisterCustomerDto.LastName)
                .NotEmpty().WithMessage("Last Name is required");

            RuleFor(x => x.RegisterCustomerDto.PhoneNumber)
                .Matches(@"^\d{10}$").WithMessage("Phone Number must be 10 digits");

            RuleFor(x => x.RegisterCustomerDto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.RegisterCustomerDto.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.RegisterCustomerDto.ConfirmPassword)
                .Equal(x => x.RegisterCustomerDto.Password)
                .WithMessage("Passwords do not match");
        }
    }
}
