using FluentValidation;

namespace Application.Features.ManageFacility.AddFacilityType
{
    public class AddFacilityTypeCommandValidator : AbstractValidator<AddFacilityTypeCommand>
    {
        public AddFacilityTypeCommandValidator()
        {
            RuleFor(x => x.FacilityTypeDto.TypeName)
                .NotEmpty().WithMessage("Facility Type Name is required.")
                .MaximumLength(50).WithMessage("Facility Type Name must not exceed 50 characters.");
        }
    }
}
