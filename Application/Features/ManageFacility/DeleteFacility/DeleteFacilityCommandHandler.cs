using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageFacility.DeleteFacility
{
    public class DeleteFacilityCommandHandler(
            IGenericRepository<Facility, int> facilityRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
            : IRequestHandler<DeleteFacilityCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            DeleteFacilityCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Check if the facility exists
                var facility = await facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
                if (facility == null)
                {
                    return Result<int>.Failure(new Error($"Facility with ID {request.FacilityId} not found."));
                }

                // Check if the facility has child facilities
                var hasChildFacilities = await facilityRepository.ExistsAsync(
                    f => f.ParentFacilityId == request.FacilityId,
                    cancellationToken);

                if (hasChildFacilities)
                {
                    return Result<int>.Failure(new Error("Cannot delete facility with child facilities. Please delete child facilities first."));
                }

                // Delete the facility
                await facilityRepository.DeleteAsync(request.FacilityId, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("Facility with ID {FacilityId} deleted successfully", request.FacilityId);
                return Result<int>.Success(request.FacilityId);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(ex, "Error deleting facility with ID {FacilityId}", request.FacilityId);
                return Result<int>.Failure(new Error($"Error deleting facility: {ex.Message}"));
            }
        }
    }
}
