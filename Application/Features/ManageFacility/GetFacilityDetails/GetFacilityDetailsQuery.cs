using Application.DTOs.FacilityDtos;
using MediatR;

namespace Application.Features.ManageFacility.GetFacilityDetails
{
    public record GetFacilityDetailsQuery : IRequest<List<FacilityDetailsDto>>
    {
    }
}
