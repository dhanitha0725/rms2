using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IRoomRepository
    {
        Task<bool> CheckAvailabilityAsync(
            int facilityId, 
            DateTime startDate, 
            DateTime endDate, 
            int quantity);

        Task<List<Room>> AssignAvailableRoomsAsync(
            int facilityId,
            int quantity,
            DateTime startDate,
            DateTime endDate);
    }
}
