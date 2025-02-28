using FluentValidation;

namespace Application.Features.ManageFacility.UpdateFacility
{
    public class UpdateFacilityCommandValidator : AbstractValidator<UpdateFacilityCommand>
    {
        public UpdateFacilityCommandValidator()
        {
            RuleFor(p => p.UpdateFacilityDto.FacilityName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(p => p.UpdateFacilityDto.FacilityType)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(p => p.UpdateFacilityDto.Location)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(p => p.UpdateFacilityDto.Description)
                .MaximumLength(200).WithMessage("{PropertyName} must not exceed 200 characters.");
        }
    }
}
