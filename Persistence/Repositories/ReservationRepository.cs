using Application.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class ReservationRepository(ReservationDbContext context) : IReservationRepository
    {
        public async Task<int> GetReservedRoomsCountAsync(
            int facilityId, 
            string roomType, 
            DateTime startDate, 
            DateTime endDate)
        {
            return await context.ReservedRooms
                .Where(r => r.Room.FacilityID == facilityId &&
                            r.Room.Type == roomType &&
                            r.StartDate < endDate &&
                            r.EndDate > startDate)
                .CountAsync();
        }

        public async Task<bool> IsPackageReservedAsync(int packageId)
        {
            return await context.ReservedPackages
                .AnyAsync(r => r.PackageID == packageId);
        }
    }
}
