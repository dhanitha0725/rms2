using FluentValidation;

namespace Application.Features.ManageFacility.UploadImages
{
    public class AddFacilityCommandValidator : AbstractValidator<AddFacilityImagesCommand>
    {
        public AddFacilityCommandValidator()
        {
            RuleFor(x => x.FacilityId).GreaterThan(0);
            RuleFor(x => x.Files)
                .NotEmpty()
                .Must(files => files.Count <= 5)
                .WithMessage("Maximum 5 images allowed");

            RuleForEach(x => x.Files)
                .Must(file => file.Length > 0)
                .WithMessage("File cannot be empty")
                .Must(file => file.Length <= 5 * 1024 * 1024)
                .WithMessage("File size exceeds 5MB limit");
        }
    }
}
