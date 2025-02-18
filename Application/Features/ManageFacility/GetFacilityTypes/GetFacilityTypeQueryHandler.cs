using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageFacility.GetFacilityTypes
{
    public class GetFacilityTypeQueryHandler(
        IGenericRepository<FacilityType, int> facilityRepository,
        IMapper mapper)
        : IRequestHandler<GetFacilityTypeQuery, Result<List<GetFacilityTypeDto>>>
    {
        public async Task<Result<List<GetFacilityTypeDto>>> Handle(
            GetFacilityTypeQuery request, 
            CancellationToken cancellationToken)
        {
            var facilityTypes = await facilityRepository.GetAllAsync();
            var facilityTypeDtos = mapper.Map<List<GetFacilityTypeDto>>(facilityTypes);

            return Result<List<GetFacilityTypeDto>>.Success(facilityTypeDtos);
        }
    }
}
