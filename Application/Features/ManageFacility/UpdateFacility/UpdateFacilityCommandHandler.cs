using System.Text.Json;
using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageFacility.UpdateFacility
{
    public class UpdateFacilityCommandHandler (
        IGenericRepository<Facility, int> facilityRepository,
        IUnitOfWork unitOfWork,
        ILogger logger) :
        IRequestHandler<UpdateFacilityCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(
            UpdateFacilityCommand request, 
            CancellationToken cancellationToken)
        {
            var facility = await facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
            if (facility == null)
            {
                logger.Warning($"Facility with id {request.FacilityId} not found.");
                return Result<string>.Failure(new Error($"Facility with id {request.FacilityId} not found."));
            }

            // Update the facility entity
            facility.FacilityName = request.UpdateFacilityDto.FacilityName;
            facility.FacilityType = request.UpdateFacilityDto.FacilityType;
            facility.Location = request.UpdateFacilityDto.Location;
            facility.Description = request.UpdateFacilityDto.Description;
            facility.Status = request.UpdateFacilityDto.Status;

            facility.Attributes = JsonSerializer.Serialize(request.UpdateFacilityDto.Attributes);

            // Save the updated facility entity to the repository
            await facilityRepository.UpdateAsync(facility, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.Information($"Facility {request.FacilityId} updated successfully.");
            return Result<string>.Success($"Facility {request.FacilityId} updated successfully.");
        }
    }
}
