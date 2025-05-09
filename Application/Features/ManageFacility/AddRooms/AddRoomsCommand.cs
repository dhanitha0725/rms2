using Application.DTOs.FacilityDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.AddRooms
{
    public record AddRoomsCommand (
        int FacilityId,
        RoomConfigurationDto RoomConfigurationDto)
        : IRequest<Result<RoomConfigurationDto>>
    {
    }
}
