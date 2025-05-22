using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManagePackages.DeletePackages
{
    public class DeletePackageCommandHandler(
            IGenericRepository<Package, int> packageRepository,
            IGenericRepository<Pricing, int> pricingRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
        : IRequestHandler<DeletePackageCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            DeletePackageCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Check if package exists
                var package = await packageRepository.GetByIdAsync(request.PackageId, cancellationToken);

                // Delete all pricing records associated with this package
                var pricings = await pricingRepository.GetAllAsync(
                    p => p.PackageID == request.PackageId,
                    cancellationToken);

                foreach (var pricing in pricings)
                {
                    await pricingRepository.DeleteAsync(pricing.PriceID, cancellationToken);
                    logger.Information("Deleted pricing {PricingId} for package {PackageId}",
                        pricing.PriceID, request.PackageId);
                }

                // Delete the package
                await packageRepository.DeleteAsync(request.PackageId, cancellationToken);
                logger.Information("Deleted package {PackageId}", request.PackageId);

                // Save changes and commit transaction
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<int>.Success(request.PackageId);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(ex, "Error deleting package {PackageId}", request.PackageId);
                return Result<int>.Failure(new Error($"Error deleting package: {ex.Message}"));
            }
        }
    }
}
