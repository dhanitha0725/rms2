using Application.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ReservationDbContext context;

        public RoomRepository(ReservationDbContext context)
        {
            this.context = context;
        }

        public async Task<int> GetRoomCountByTypeAsync(int facilityId, string roomType)
        {
            return await context.Rooms
                .Where(r => r.FacilityID == facilityId && r.Type == roomType)
                .CountAsync();
        }

        public async Task<int> ReserveRoomsAsync(
            string roomType,
            int facilityId,
            DateTime startDate,
            DateTime endDate,
            int quantity)
        {
            var rooms = await context.Rooms
                .FromSqlInterpolated($@"
                            SELECT TOP {quantity} *
                            FROM Rooms WITH (UPDLOCK)
                            WHERE Type = {roomType}
                            AND Status = 'Available'")
                .ToListAsync();

            // set rooms as reserved
            rooms.ForEach(r => r.Status = "Reserved");
            await context.SaveChangesAsync();

            return rooms.Count;
        }
    }
}
