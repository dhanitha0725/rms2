using Application.DTOs.PackageDto;
using Domain.Common;
using Domain.Entities;
using Application.Abstractions.Interfaces;
using MediatR;

namespace Application.Features.ManagePackages.GetRoomPricing
{
    public class GetRoomPricingQueryHandler(
        IGenericRepository<RoomPricing, int> roomPricingRepository,
        IGenericRepository<RoomType, int> roomTypeRepository,
        IGenericRepository<Facility, int> facilityRepository,
        IGenericRepository<Room, int> roomRepository
    ) : IRequestHandler<GetRoomPricingQuery, Result<List<RoomPricingTableDto>>>
    {
        public async Task<Result<List<RoomPricingTableDto>>> Handle(
            GetRoomPricingQuery request,
            CancellationToken cancellationToken)
        {
            // Get all facilities, rooms, room types, and room pricing
            var facilities = await facilityRepository.GetAllAsync(cancellationToken);
            var rooms = await roomRepository.GetAllAsync(cancellationToken);
            var roomTypes = await roomTypeRepository.GetAllAsync(cancellationToken);
            var roomPricing = await roomPricingRepository.GetAllAsync(cancellationToken);

            var result = new List<RoomPricingTableDto>();

            foreach (var facility in facilities)
            {
                // Get all rooms for this facility
                var facilityRooms = rooms.Where(r => r.FacilityID == facility.FacilityID).ToList();
                if (!facilityRooms.Any())
                    continue; // Ignore facilities with no rooms

                // Get all room types for this facility
                var facilityRoomTypeIds = facilityRooms.Select(r => r.RoomTypeID).Distinct();

                foreach (var roomTypeId in facilityRoomTypeIds)
                {
                    var roomType = roomTypes.FirstOrDefault(rt => rt.RoomTypeID == roomTypeId);
                    if (roomType == null)
                        continue;

                    // Count rooms of this type in this facility
                    var roomCount = facilityRooms.Count(r => r.RoomTypeID == roomTypeId);

                    // Get pricing for this facility and room type
                    var pricingDict = roomPricing
                        .Where(rp => rp.FacilityID == facility.FacilityID && rp.RoomTypeID == roomTypeId)
                        .ToDictionary(rp => rp.Sector, rp => rp.Price);

                    result.Add(new RoomPricingTableDto
                    {
                        RoomTypeId = roomType.RoomTypeID,
                        RoomTypeName = roomType.TypeName,
                        FacilityName = facility.FacilityName,
                        TotalRooms = roomCount,
                        Pricings = pricingDict 
                    });
                }
            }

            return Result<List<RoomPricingTableDto>>.Success(result);
        }
    }
}