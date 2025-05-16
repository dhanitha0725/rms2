using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.GetRoomTypes
{
    public class GetRoomTypesQuery : IRequest<Result<List<RoomTypesDto>>>
    {
    }
}
