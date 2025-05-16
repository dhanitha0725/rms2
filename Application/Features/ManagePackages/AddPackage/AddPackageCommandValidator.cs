using Application.Abstractions.Interfaces;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.ManagePackages.AddPackage
{
    public class AddPackageCommandValidator : AbstractValidator<AddPackageCommand>
    {
        private static readonly HashSet<string> ValidSectors = new(StringComparer.OrdinalIgnoreCase)
            {
                "corporate", "public", "private"
            };

        public AddPackageCommandValidator(IGenericRepository<Facility, int> facilityRepo)
        {
            RuleFor(x => x.FacilityId)
                .MustAsync(async (id, ct) => await facilityRepo.ExistsAsync(f => f.FacilityID == id, ct))
                .WithMessage("Facility does not exist");

            RuleFor(x => x.PackageDto.PackageName)
                .NotEmpty().WithMessage("Package name is required")
                .MaximumLength(100).WithMessage("Package name cannot exceed 100 characters");

            RuleFor(x => x.PackageDto.Duration)
                .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be positive");

            RuleFor(x => x.PackageDto.Pricings)
                .NotEmpty().WithMessage("At least one pricing sector required")
                .Must(p => p.Keys.Any(k => ValidSectors.Contains(k)))
                .WithMessage("Invalid sector name(s). Valid values: Corporate, Public, Private")
                .Must(p => p.Values.All(v => v > 0))
                .WithMessage("All prices must be positive");
        }
    }
}
