using Application.Abstractions.Interfaces;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.ManagePackages.DeletePackages
{
    public class DeletePackageCommandValidator : AbstractValidator<DeletePackageCommand>
    {
        public DeletePackageCommandValidator(
            IGenericRepository<Package, int> packageRepository,
            IGenericRepository<ReservedPackage, int> reservedPackageRepository)
        {
            RuleFor(x => x.PackageId)
                .NotEmpty().WithMessage("Package ID is required")
                .MustAsync(async (id, cancellationToken) =>
                    await packageRepository.ExistsAsync(p => p.PackageID == id, cancellationToken))
                .WithMessage(x => $"Package with ID {x.PackageId} does not exist");

            RuleFor(x => x.PackageId)
                .MustAsync(async (id, cancellationToken) =>
                    !await reservedPackageRepository.ExistsAsync(rp => rp.PackageID == id, cancellationToken))
                .WithMessage(x => $"Package with ID {x.PackageId} cannot be deleted as it is associated with reservations");
        }
    }
}
