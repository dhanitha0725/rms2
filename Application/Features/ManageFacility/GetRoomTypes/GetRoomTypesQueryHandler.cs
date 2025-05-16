using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using Application.Abstractions.Interfaces;
using MediatR;

namespace Application.Features.ManageFacility.GetRoomTypes
{
    public class GetRoomTypesQueryHandler(
        IGenericRepository<RoomType, int> roomTypeRepository) : IRequestHandler<GetRoomTypesQuery, Result<List<RoomTypesDto>>>
    {

        public async Task<Result<List<RoomTypesDto>>> Handle(GetRoomTypesQuery request, CancellationToken cancellationToken)
        {
            var roomTypes = await roomTypeRepository.GetAllAsync(cancellationToken);

            var roomTypeDto = roomTypes.Select(rt => new RoomTypesDto
            {
                RoomTypeId = rt.RoomTypeID,
                RoomTypeName = rt.TypeName
            }).ToList();

            return Result<List<RoomTypesDto>>.Success(roomTypeDto);
        }
    }   
}
