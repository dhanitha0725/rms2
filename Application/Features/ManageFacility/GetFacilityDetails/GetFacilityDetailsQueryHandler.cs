using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageFacility.GetFacilityDetails
{
    public class GetFacilityDetailsQueryHandler(
            IGenericRepository<Facility, int> facilityRepository,
            IGenericRepository<FacilityType, int> facilityTypeRepository)
            : IRequestHandler<GetFacilityDetailsQuery, List<FacilityDetailsDto>>
    {
        public async Task<List<FacilityDetailsDto>> Handle(
            GetFacilityDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var facilities = await facilityRepository.GetAllAsync(cancellationToken);
            return facilities.Select(f => new FacilityDetailsDto
            {
                FacilityId = f.FacilityID,
                FacilityName = f.FacilityName,
                Location = f.Location,
                Status = f.Status
            }).ToList();
        }
    }
}