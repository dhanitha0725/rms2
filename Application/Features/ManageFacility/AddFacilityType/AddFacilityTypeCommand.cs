using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.AddFacilityType
{
    public record AddFacilityTypeCommand (AddFacilityTypeDto FacilityTypeDto) : IRequest<Result<int>>
    {

    }
}
