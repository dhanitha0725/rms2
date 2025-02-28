using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageFacility.GetFacilityNames
{
    public class GetFacilityNamesQueryHandler (
        IGenericRepository<Facility, int> facilityRepository) 
        : IRequestHandler<GetFacilityNamesQuery, Result<List<FacilityNamesSelectDto>>>
    {
        public async Task<Result<List<FacilityNamesSelectDto>>> Handle(
            GetFacilityNamesQuery request,
            CancellationToken cancellationToken)
        {
            var facilities = await facilityRepository.GetAllAsync(cancellationToken);
            var facilityDtos = facilities.Select(facility => new FacilityNamesSelectDto
            {
                FacilityID = facility.FacilityID,
                FacilityName = facility.FacilityName
            }).ToList();

            return Result<List<FacilityNamesSelectDto>>.Success(facilityDtos);
        }
    }
}
