using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.GetFacilityNames
{
    public record GetFacilityNamesQuery : IRequest<List<FacilityNamesSelectDto>>
    {
    }
}
