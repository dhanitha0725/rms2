using FluentValidation;

namespace Application.Features.ManageFacility.UploadImages
{
    public class AddFacilityCommandValidator : AbstractValidator<AddFacilityImagesCommand>
    {
        public AddFacilityCommandValidator()
        {
            RuleFor(x => x.FacilityId).GreaterThan(0);
            RuleFor(x => x.AddFacilityImagesDto.Images)
                .NotEmpty()
                .Must(images => images.Count <= 5)
                .WithMessage("Maximum 5 images allowed");
        }
    }
}
