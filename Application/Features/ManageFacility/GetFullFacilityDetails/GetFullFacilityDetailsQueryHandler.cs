using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageFacility.GetFullFacilityDetails
{
    public class GetFullFacilityDetailsQueryHandler(
            IGenericRepository<Facility, int> facilityRepository)
            : IRequestHandler<GetFullFacilityDetailsQuery, Result<FullFacilityDetailsDto>>
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

            var facilityDetailsDto = new FullFacilityDetailsDto
            {
                FacilityID = facility.FacilityID,
                FacilityName = facility.FacilityName,
                FacilityType = facility.FacilityType?.TypeName ?? string.Empty, 
                Location = facility.Location,
                Description = facility.Description,
                Status = facility.Status ?? string.Empty,
                CreatedDate = facility.CreatedDate,
                Attributes = facility.Attributes ?? new List<string>(),
                ChildFacilities = facility.ChildFacilities != null
                    ? facility.ChildFacilities.Select(cf => new ChildFacilityDto
                    {
                        ChildrenFacilityId = cf.FacilityID,
                        Name = cf.FacilityName,
                        Type = cf.FacilityType?.TypeName ?? string.Empty 
                    }).ToList()
                    : new List<ChildFacilityDto>(),
                Images = facility.Images != null
                    ? facility.Images.Select(i => i.ImageUrl).ToList()
                    : new List<string>()
            };

            return Result<FullFacilityDetailsDto>.Success(facilityDetailsDto);
        }
    }
}
