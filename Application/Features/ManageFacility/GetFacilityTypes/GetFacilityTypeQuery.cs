using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.GetFacilityTypes
{
    public class GetFacilityTypeQuery: IRequest<Result<List<GetFacilityTypeDto>>>;
}
