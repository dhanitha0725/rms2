using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.FacilityCardDetails
{
    public class GetFacilityCardsQuery : IRequest<Result<List<FacilityCardDto>>>
    {
    }
}
