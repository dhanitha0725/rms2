using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageFacility.AddFacilityType
{
    public class AddFacilityTypeCommandHandler(
            IGenericRepository<FacilityType, int> facilityTypeRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
            : IRequestHandler<AddFacilityTypeCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            AddFacilityTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Check if a facility type with the same name already exists
                var facilityTypeExists = await facilityTypeRepository.ExistsAsync(
                    ft => ft.TypeName.ToLower() == request.FacilityTypeDto.TypeName.ToLower(),
                    cancellationToken);

                if (facilityTypeExists)
                {
                    logger.Warning("Attempted to add duplicate facility type: {TypeName}",
                        request.FacilityTypeDto.TypeName);
                    return Result<int>.Failure(new Error($"A facility type with the name '{request.FacilityTypeDto.TypeName}' already exists."));
                }

                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var newFacilityType = new FacilityType
                {
                    TypeName = request.FacilityTypeDto.TypeName
                };

                await facilityTypeRepository.AddAsync(newFacilityType, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("New Facility Type Added with Id: {FacilityTypeId}", newFacilityType.FacilityTypeId);

                return Result<int>.Success(newFacilityType.FacilityTypeId);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(ex, "Error adding new Facility Type");
                throw;
            }
        }
    }
}
