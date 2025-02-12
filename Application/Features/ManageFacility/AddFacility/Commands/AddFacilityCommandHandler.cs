using System.Text.Json;
using MediatR;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Serilog;

namespace Application.Features.ManageFacility.AddFacility.Commands
{
    public class AddFacilityCommandHandler (
        IGenericRepository<Facility, int> facilityRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ILogger logger) :
        IRequestHandler<AddFacilityCommand, Result<int>>
    {

        public async Task<Result<int>> Handle(
            AddFacilityCommand request,
            CancellationToken cancellationToken)
        {
            var facility = mapper.Map<Facility>(request.FacilityDto);

            facility.CreatedDate = DateTime.UtcNow;

            // serialize attributes
            facility.Attributes = JsonSerializer.Serialize(
                request.FacilityDto.Attributes);

            await facilityRepository.AddAsync(facility, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.Information("New Facility Added with Id: {FacilityId}", facility.FacilityID);

            return Result<int>.Success(facility.FacilityID);
        }
    }
}
