using Application.Abstractions.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class RoomRepository (ReservationDbContext context) : IRoomRepository
    {
        public async Task<bool> CheckAvailabilityAsync(
            int facilityId, 
            DateTime startDate,
            DateTime endDate, 
            int quantity)
        {
            var availableRooms = await context.Rooms
                .FromSqlInterpolated($@"
                    SELECT * FROM Rooms WITH (UPDLOCK)
                    WHERE FacilityID = {facilityId}
                    AND RoomID NOT IN(
                        SELECT RoomID FROM ReservedRooms
                        INNER JOIN Reservations ON Reservations.ReservationID = ReservedRooms.ReservationID
                        WHERE Reservations.StartDate < {endDate} AND Reservations.EndDate > {startDate})")
                .Take(quantity)
                .ToListAsync();

            return availableRooms.Count >= quantity;
        }

        public async Task<List<Room>> AssignAvailableRoomsAsync(
            int facilityId, 
            int quantity, 
            DateTime startDate, 
            DateTime endDate)
        {
            return await context.Rooms
                .FromSqlInterpolated($@"
                    SELECT TOP {quantity} * FROM Rooms WITH (UPDLOCK)
                    WHERE FacilityID = {facilityId}
                    AND RoomID NOT IN (
                        SELECT RoomID FROM ReservedRooms
                        INNER JOIN Reservations ON Reservation.ReservationID = ReservedRooms.ReservationID
                        WHERE Reservations.StartDate < {endDate} AND Reservations.EndDate > {startDate})")
                .ToListAsync();
        }
    }
}
