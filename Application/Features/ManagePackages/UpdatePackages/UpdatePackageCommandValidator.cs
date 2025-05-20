using Application.Abstractions.Interfaces;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.ManagePackages.UpdatePackages;

public class UpdatePackageCommandValidator : AbstractValidator<UpdatePackageCommand>
{
    private static readonly HashSet<string> ValidSectors = new(StringComparer.OrdinalIgnoreCase)
    {
        "corporate", "public", "private"
    };

    public UpdatePackageCommandValidator(IGenericRepository<Package, int> packageRepository)
    {
        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("Package ID is required")
            .MustAsync(async (id, cancellationToken) =>
                await packageRepository.ExistsAsync(p => p.PackageID == id, cancellationToken))
            .WithMessage(x => $"Package with ID {x.PackageId} does not exist");

        When(x => x.PackageDto != null, () =>
        {
            RuleFor(x => x.PackageDto.PackageName)
                .NotEmpty().WithMessage("Package name is required")
                .MaximumLength(100).WithMessage("Package name cannot exceed 100 characters")
                .When(x => x.PackageDto.PackageName != null);

            RuleFor(x => x.PackageDto.Duration)
                .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be positive")
                .When(x => x.PackageDto.Duration.HasValue);

            When(x => x.PackageDto.Pricing != null, () =>
            {
                RuleFor(x => x.PackageDto.Pricing)
                    .Must(p => p.Keys.All(k => ValidSectors.Contains(k)))
                    .WithMessage("Invalid sector name(s). Valid values: Corporate, Public, Private")
                    .Must(p => p.Values.All(v => v > 0))
                    .WithMessage("All prices must be positive");
            });
        });
    }
}
