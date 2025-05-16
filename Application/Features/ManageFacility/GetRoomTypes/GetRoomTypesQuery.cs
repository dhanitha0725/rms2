using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.GetRoomTypes
{
    public class GetRoomTypesQuery : IRequest<Result<List<RoomTypesDto>>>
    {
    }
}
