using MediatR;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Serilog;

namespace Application.Features.ManageFacility.AddFacility.Commands;

public class AddFacilityCommandHandler (
    IGenericRepository<Facility, int> facilityRepository,
    IGenericRepository<FacilityType, int> facilityTypeRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ILogger logger) 
    : IRequestHandler<AddFacilityCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        AddFacilityCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Check if a facility with the same name and location already exists
            var facilityExists = await facilityRepository.ExistsAsync(
                f => f.FacilityName == request.FacilityDto.FacilityName &&
                     f.Location == request.FacilityDto.Location,
                cancellationToken);

            if (facilityExists)
            {
                return Result<int>.Failure(new Error("A facility with the same name and location already exists"));
            }

            // check facility type exists
            var typeExists = await facilityTypeRepository.ExistsAsync(
                ft => ft.FacilityTypeId == request.FacilityDto.FacilityTypeId,
                cancellationToken);

            if (!typeExists)
            {
                return Result<int>.Failure(new Error("Invalid facility type"));
            }

            // validate parent facility if provided
            if (request.FacilityDto.ParentFacilityId.HasValue)
            {
                var parentExists = await facilityRepository.ExistsAsync(
                    f => f.FacilityID == request.FacilityDto.ParentFacilityId.Value,
                    cancellationToken);

                if (!parentExists)
                {
                    return Result<int>.Failure(new Error("Parent facility does not exist"));
                }
            }

            // Create facility
            var facility = mapper.Map<Facility>(request.FacilityDto);
            //facility.FacilityTypeId = request.FacilityDto.FacilityTypeId;
            facility.CreatedDate = DateTime.UtcNow;


            await facilityRepository.AddAsync(facility, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            logger.Information("New Facility Added with Id: {FacilityId}", facility.FacilityID);
            return Result<int>.Success(facility.FacilityID);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            logger.Error(ex, "Error adding new facility");
            return Result<int>.Failure(new Error($"Error adding new facility: {ex.Message}"));
        }
    }
}