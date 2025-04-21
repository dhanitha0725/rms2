using FluentValidation;

namespace Application.Features.ManageFacility.AddFacility.Commands;

public class AddFacilityCommandValidator : AbstractValidator<AddFacilityCommand>
{
    public AddFacilityCommandValidator()
    {
        RuleFor(x => x.FacilityDto.FacilityName)
            .NotEmpty().WithMessage("Facility name is required.");

        RuleFor(x => x.FacilityDto.FacilityTypeId)
            .NotEmpty().WithMessage("Invalid facility type");

        RuleFor(x => x.FacilityDto.Attributes)
            .NotEmpty().WithMessage("Attributes are required.");

        RuleFor(x => x.FacilityDto.Location)
            .NotEmpty().WithMessage("Location is required.");

        RuleFor(x => x.FacilityDto.ParentFacilityId)
            .GreaterThan(0).When(x => x.FacilityDto.ParentFacilityId.HasValue)
            .WithMessage("Invalid parent facility ID");
    }
}