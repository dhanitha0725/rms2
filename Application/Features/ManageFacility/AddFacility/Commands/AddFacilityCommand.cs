using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.AddFacility.Commands
{
    public record AddFacilityCommand (AddFacilityDto FacilityDto) : IRequest<Result<int>>
    {
    }
}
