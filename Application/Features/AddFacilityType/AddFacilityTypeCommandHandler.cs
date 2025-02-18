using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.AddFacilityType
{
    public class AddFacilityTypeCommandHandler (
        IGenericRepository<FacilityType, int> facilityTypeRepository,
        IUnitOfWork unitOfWork,
        ILogger logger) 
        : IRequestHandler<AddFacilityTypeCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            AddFacilityTypeCommand request, 
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var newFacilityType = new FacilityType
                {
                    TypeName = request.FacilityTypeDto.TypeName
                };

                await facilityTypeRepository.AddAsync(newFacilityType, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

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
