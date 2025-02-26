using System.Text.Json;
using FluentValidation;

namespace Application.Features.ManageFacility.AddFacility.Commands
{
    public class AddFacilityCommandValidator : AbstractValidator<AddFacilityCommand>
    {
        public AddFacilityCommandValidator()
        {
            RuleFor(x => x.FacilityDto).NotNull().WithMessage("Facility details are required.");

            When(x => true, () =>
            {
                RuleFor(x => x.FacilityDto.FacilityName)
                    .NotEmpty().WithMessage("Facility name is required.");

                //RuleFor(x => x.FacilityDto.AttributesJson)
                //    .NotEmpty().WithMessage("Attributes are required.")
                //    .Must(BeValidJson).WithMessage("Attributes must be a valid JSON object.");

                RuleFor(x => x.FacilityDto.FacilityTypeId)
                    .NotEmpty().WithMessage("Facility type is required.");

                RuleFor(x => x.FacilityDto.Attributes)
                    .NotEmpty().WithMessage("Attributes are required.");

                RuleFor(x => x.FacilityDto.Location)
                    .NotEmpty().WithMessage("Location is required.");

                RuleFor(x => x.FacilityDto.Description)
                    .NotEmpty().WithMessage("Description is required.");

                RuleFor(x => x.FacilityDto.Status)
                    .NotEmpty().WithMessage("Status is required.");

                //RuleFor(x => x.FacilityDto.Images)
                //    .Must(images => images is not { Count: > 5 })
                //    .WithMessage("Maximum of 5 images are allowed.");
            });
        }

        //private bool BeValidJson(string json)
        //{
        //    try
        //    {
        //        JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        //        return true;
        //    }
        //    catch (JsonException)
        //    {
        //        return false;
        //    }
        //}

    }
}
