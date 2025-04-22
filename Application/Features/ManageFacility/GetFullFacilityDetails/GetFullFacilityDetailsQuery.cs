using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.GetFullFacilityDetails
{
    public class GetFullFacilityDetailsQuery : IRequest<Result<FullFacilityDetailsDto>>
    {
        public int FacilityId { get; set; }
    }
}
