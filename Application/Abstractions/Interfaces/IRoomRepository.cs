namespace Application.Abstractions.Interfaces
{
    public interface IRoomRepository
    {
        Task<int> GetRoomCountByTypeAsync(int facilityId, string roomType);
        Task<int> ReserveRoomsAsync(string roomType, int facilityId, DateTime startDate, DateTime endDate, int quantity);
    }
}
