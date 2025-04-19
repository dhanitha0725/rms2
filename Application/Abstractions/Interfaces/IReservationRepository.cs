using Domain.Entities;

namespace Application.Abstractions.Interfaces
{
    public interface IReservationRepository
    {
        Task<int> GetReservedRoomsCountAsync(int facilityId, string roomType, DateTime startDate, DateTime endDate);
        Task<bool> IsPackageReservedAsync(int packageId);
    }
}
