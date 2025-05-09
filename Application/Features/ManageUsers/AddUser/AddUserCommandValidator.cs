using FluentValidation;

namespace Application.Features.ManageUsers.AddUser
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        private readonly List<string> _allowedRoles = ["Admin", "Employee", "Accountant", "Hostel"];

        public AddUserCommandValidator()
        {
            RuleFor(x => x.AddUserDto.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters");
            
            RuleFor(x => x.AddUserDto.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");
            
            RuleFor(x => x.AddUserDto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.AddUserDto.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => _allowedRoles.Contains(role))
                .WithMessage($"Role must be one of the following: {string.Join(", ", _allowedRoles)}");

            RuleFor(x => x.AddUserDto.PhoneNumber)
                .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits");
        }
    }
}
