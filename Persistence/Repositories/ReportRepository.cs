using Application.Abstractions.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class ReportRepository(ReservationDbContext context) : IReportRepository
    {

        public async Task<List<FinancialReport>> GetFinancialReportAsync(
            DateTime startDate,
            DateTime endDate,
            IEnumerable<int>? facilityIds,
            CancellationToken cancellationToken = default)
        {
            // Convert to UTC and ensure we're working with a date without time component
            startDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc);

            // Retrieve all facility IDs we need to report on
            var facilities = await context.Facilities
                .Where(f => facilityIds == null || !facilityIds.Any() || facilityIds.Contains(f.FacilityID))
                .Select(f => new { f.FacilityID, f.FacilityName })
                .ToListAsync(cancellationToken);

            // Get eligible reservations
            var eligibleReservations = await context.Reservations
                .Where(r => (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Completed)
                           && r.EndDate >= startDate && r.EndDate <= endDate)
                .Select(r => new { r.ReservationID })
                .ToListAsync(cancellationToken);

            // Get room-based reservations
            var roomReservations = await context.ReservedRooms
                .Where(rr => eligibleReservations.Select(r => r.ReservationID).Contains(rr.ReservationID))
                .Join(context.Rooms,
                    rr => rr.RoomID,
                    rm => rm.RoomID,
                    (rr, room) => new
                    {
                        ReservationId = rr.ReservationID,
                        FacilityId = room.FacilityID
                    })
                .ToListAsync(cancellationToken);

            // Get package-based reservations
            var packageReservations = await context.ReservedPackages
                .Where(rp => eligibleReservations.Select(r => r.ReservationID).Contains(rp.ReservationID))
                .Join(context.Packages,
                    rp => rp.PackageID,
                    pkg => pkg.PackageID,
                    (rp, pkg) => new
                    {
                        ReservationId = rp.ReservationID,
                        FacilityId = pkg.FacilityID
                    })
                .ToListAsync(cancellationToken);

            // Combine room and package reservations in memory (not in the query)
            var allReservations = roomReservations.Union(packageReservations)
                .GroupBy(r => new { r.ReservationId, r.FacilityId })
                .Select(g => new
                {
                    ReservationId = g.Key.ReservationId,
                    FacilityId = g.Key.FacilityId
                })
                .ToList();

            // Get payment data
            var payments = await context.Payments
                .Where(p => p.Status == "Completed" &&
                       eligibleReservations.Select(r => r.ReservationID).Contains(p.ReservationID))
                .GroupBy(p => p.ReservationID)
                .Select(g => new
                {
                    ReservationId = g.Key,
                    TotalPaid = g.Sum(p => p.AmountPaid ?? 0)
                })
                .ToListAsync(cancellationToken);

            // Build the report in memory
            var report = facilities.Select(f =>
            {
                var facilityReservations = allReservations
                    .Where(r => r.FacilityId == f.FacilityID)
                    .ToList();

                var totalRevenue = facilityReservations
                    .Join(payments,
                        r => r.ReservationId,
                        p => p.ReservationId,
                        (r, p) => p.TotalPaid)
                    .Sum();

                return new FinancialReport
                {
                    FacilityId = f.FacilityID,
                    FacilityName = f.FacilityName,
                    TotalReservations = facilityReservations.Count,
                    TotalRevenue = totalRevenue
                };
            })
            .OrderBy(r => r.FacilityName)
            .ToList();

            return report;
        }

        public async Task<List<ReservationReport>> GetReservationReportAsync(
            DateTime startDate,
            DateTime endDate,
            IEnumerable<int>? facilityIds,
            CancellationToken cancellationToken = default)
        {
            // Convert to UTC and ensure we're working with a date without time component
            startDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            endDate = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc);

            // Retrieve all facility IDs we need to report on
            var facilities = await context.Facilities
                .Where(f => facilityIds == null || !facilityIds.Any() || facilityIds.Contains(f.FacilityID))
                .Select(f => new { f.FacilityID, f.FacilityName })
                .ToListAsync(cancellationToken);

            // Get reservations within date range
            var reservationsInRange = await context.Reservations
                .Where(r => r.EndDate >= startDate && r.EndDate <= endDate)
                .Select(r => new { r.ReservationID, r.Status })
                .ToListAsync(cancellationToken);

            // Get room-based reservations
            var roomReservations = await context.ReservedRooms
                .Where(rr => reservationsInRange.Select(r => r.ReservationID).Contains(rr.ReservationID))
                .Join(context.Rooms,
                    rr => rr.RoomID,
                    rm => rm.RoomID,
                    (rr, room) => new
                    {
                        ReservationId = rr.ReservationID,
                        FacilityId = room.FacilityID
                    })
                .ToListAsync(cancellationToken);

            // Get package-based reservations
            var packageReservations = await context.ReservedPackages
                .Where(rp => reservationsInRange.Select(r => r.ReservationID).Contains(rp.ReservationID))
                .Join(context.Packages,
                    rp => rp.PackageID,
                    pkg => pkg.PackageID,
                    (rp, pkg) => new
                    {
                        ReservationId = rp.ReservationID,
                        FacilityId = pkg.FacilityID
                    })
                .ToListAsync(cancellationToken);

            // Combine room and package reservations in memory
            var allReservations = roomReservations.Union(packageReservations)
                .GroupBy(r => new { r.ReservationId, r.FacilityId })
                .Select(g => new
                {
                    ReservationId = g.Key.ReservationId,
                    FacilityId = g.Key.FacilityId
                })
                .ToList();

            // Build the report in memory
            var report = facilities.Select(f =>
            {
                var facilityReservations = allReservations
                    .Where(r => r.FacilityId == f.FacilityID)
                    .ToList();

                var reservationIds = facilityReservations
                    .Select(r => r.ReservationId)
                    .Distinct()
                    .ToList();

                var completedReservationCount = reservationsInRange
                    .Where(r => reservationIds.Contains(r.ReservationID) &&
                           r.Status == ReservationStatus.Completed)
                    .Count();

                return new ReservationReport
                {
                    FacilityId = f.FacilityID,
                    FacilityName = f.FacilityName,
                    TotalReservations = reservationIds.Count,
                    TotalCompletedReservations = completedReservationCount
                };
            })
            .OrderBy(r => r.FacilityName)
            .ToList();

            return report;
        }
    }
}
