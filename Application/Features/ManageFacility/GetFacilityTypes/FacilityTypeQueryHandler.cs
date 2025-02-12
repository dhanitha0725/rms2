using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.GetFacilityTypes
{
    public class FacilityTypeQueryHandler(
        IFacilityRepository facilityRepository) 
        : IRequestHandler<FacilityTypeQuery, Result<List<FacilityTypeDto>>>
    {
        public async Task<Result<List<FacilityTypeDto>>> Handle(
            FacilityTypeQuery request, 
            CancellationToken cancellationToken)
        {
            return await facilityRepository.GetFacilityTypesAsync();
        }
    }
}
