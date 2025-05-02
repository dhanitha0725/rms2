using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.AddFacility;

public class AddFacilityCommand : IRequest<Result<int>>
{
    public AddFacilityDto FacilityDto { get; set; }
}