using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.SelectedFacilityDetails
{
    public class GetSelectedFacilityDetailsQuery : IRequest<Result<SelectedFacilityDetailsDto>>
    {
        public int FacilityId { get; set; }
    }
}
