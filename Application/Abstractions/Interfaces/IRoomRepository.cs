using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IRoomRepository : IGenericRepository<Room, int>
    {
        Task<List<RoomPricing>> GetRoomPricingWithRoomTypeAsync(int facilityId, List<int> roomTypeIds, CancellationToken cancellationToken);
    }
}
