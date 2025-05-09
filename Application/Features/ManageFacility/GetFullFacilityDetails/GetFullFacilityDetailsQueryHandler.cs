using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageFacility.GetFullFacilityDetails
{
    public class GetFullFacilityDetailsQueryHandler(
        IGenericRepository<Facility, int> facilityRepository,
        IMapper mapper) : 
        IRequestHandler<GetFullFacilityDetailsQuery, Result<FullFacilityDetailsDto>>
    {
        public async Task<Result<FullFacilityDetailsDto>> Handle(
            GetFullFacilityDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var facility = await facilityRepository.GetByIdWithIncludeAsync(
                request.FacilityId,
                f => f.FacilityType,
                f => f.ChildFacilities,
                f => f.Images
            );

            if (facility == null)
            {
                return Result<FullFacilityDetailsDto>.Failure(new Error("Facility not found"));
            }

            var facilityDetailsDto = mapper.Map<FullFacilityDetailsDto>(facility);
            return Result<FullFacilityDetailsDto>.Success(facilityDetailsDto);

        }
    }
}
