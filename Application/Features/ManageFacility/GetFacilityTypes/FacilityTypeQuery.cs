using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.GetFacilityTypes
{
    public record FacilityTypeQuery : IRequest<Result<List<FacilityTypeDto>>>;
}
