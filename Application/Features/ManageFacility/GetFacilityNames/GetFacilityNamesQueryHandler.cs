using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageFacility.GetFacilityNames
{
    public class GetFacilityNamesQueryHandler(
            IGenericRepository<Facility, int> facilityRepository)
            : IRequestHandler<GetFacilityNamesQuery, List<FacilityNamesSelectDto>>
    {
        public async Task<List<FacilityNamesSelectDto>> Handle(
            GetFacilityNamesQuery request,
            CancellationToken cancellationToken)
        {
            var facilities = await facilityRepository.GetAllAsync(cancellationToken);
            var facilityDtos = facilities.Select(facility => new FacilityNamesSelectDto
            {
                FacilityID = facility.FacilityID,
                FacilityName = facility.FacilityName
            }).ToList();

            return facilityDtos;
        }
    }
}
