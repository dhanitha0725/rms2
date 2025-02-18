using FluentValidation;

namespace Application.Features.ManageFacility.AddFacility.Commands
{
    public class AddFacilityCommandValidator : AbstractValidator<AddFacilityCommand>
    {
        public AddFacilityCommandValidator()
        {
            RuleFor(x => x.FacilityDto).NotNull().WithMessage("Facility details are required.");

            When(x => x.FacilityDto != null, () =>
            {
                RuleFor(x => x.FacilityDto.FacilityName)
                    .NotEmpty().WithMessage("Facility name is required.");

                RuleFor(x => x.FacilityDto.Attributes)
                    .NotNull().WithMessage("Attributes are required.");

                RuleFor(x => x.FacilityDto.Location)
                    .NotEmpty().WithMessage("Location is required.");

                RuleFor(x => x.FacilityDto.Description)
                    .NotEmpty().WithMessage("Description is required.");

                RuleFor(x => x.FacilityDto.Status)
                    .NotEmpty().WithMessage("Status is required.");
            });
        }
    }
}
