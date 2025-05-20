using Application.Abstractions.Interfaces;
using Application.DTOs.PackageDto;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManagePackages.UpdatePackages
{
    public class UpdatePackageCommandHandler(
            IGenericRepository<Package, int> packageRepository,
            IGenericRepository<Pricing, int> pricingRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
            : IRequestHandler<UpdatePackageCommand, Result<UpdatePackageDto>>
    {
        public async Task<Result<UpdatePackageDto>> Handle(
            UpdatePackageCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Get existing package
                var package = await packageRepository.GetByIdAsync(request.PackageId, cancellationToken);

                // Update basic package properties
                package.PackageName = request.PackageDto.PackageName ?? package.PackageName;
                package.Duration = request.PackageDto.Duration ?? package.Duration;

                // Update package
                await packageRepository.UpdateAsync(package, cancellationToken);

                // Update or create pricing if provided
                if (request.PackageDto.Pricing != null && request.PackageDto.Pricing.Count > 0)
                {
                    // Get existing pricing
                    var existingPricings = await pricingRepository.GetAllAsync(
                        p => p.PackageID == request.PackageId,
                        cancellationToken);

                    foreach (var sectorPrice in request.PackageDto.Pricing)
                    {
                        var sector = sectorPrice.Key;
                        var price = sectorPrice.Value;

                        // Find existing pricing for this sector or null if it doesn't exist
                        var existingPricing = existingPricings.FirstOrDefault(p => p.Sector == sector);

                        if (existingPricing != null)
                        {
                            // Update existing pricing
                            existingPricing.Price = price;
                            await pricingRepository.UpdateAsync(existingPricing, cancellationToken);
                            logger.Information("Updated pricing for package {PackageId}, sector {Sector}: {Price}",
                                request.PackageId, sector, price);
                        }
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("Package {PackageId} updated successfully", request.PackageId);
                return Result<UpdatePackageDto>.Success(request.PackageDto);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "An error occurred while updating package {PackageId}", request.PackageId);
                return Result<UpdatePackageDto>.Failure(new Error($"Failed to update package: {e.Message}"));
            }
        }
    }
}
