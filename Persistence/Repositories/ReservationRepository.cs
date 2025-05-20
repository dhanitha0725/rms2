using System.Linq.Expressions;
using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class ReservationRepository(ReservationDbContext context) : IReservationRepository
    {
        public Task<IEnumerable<Reservation>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reservation>> GetAllAsync(Expression<Func<Reservation, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation> AddAsync(Reservation entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Reservation entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Reservation, bool>> predicate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation?> AddRangeAsync(IEnumerable<Reservation> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation?> GetByIdWithIncludeAsync(int id, params Expression<Func<Reservation, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<Reservation?> GetByIdWithQueryAsync(Func<IQueryable<Reservation>, IQueryable<Reservation>> queryBuilder, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Reservation> GetQuery()
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<Reservation, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<Reservation, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Reservation?> GetReservationDetailsAsync(int reservationId, CancellationToken cancellationToken = default)
        {
            return await context.Reservations
                .Where(r => r.ReservationID == reservationId)
                    .Include(r => r.ReservationUserDetail)
                    .Include(r => r.Payments)!
                        .ThenInclude(p => p.Documents)
                    .Include(r => r.Documents)
                    .Include(r => r.ReservedPackages)
                        .ThenInclude(rp => rp.Package)
                            .ThenInclude(p => p.Facility)
                    .Include(r => r.ReservedRooms)!
                        .ThenInclude(rr => rr.Room)
                            .ThenInclude(room => room.RoomType)
                    .Include(r => r.ReservedRooms)!
                        .ThenInclude(rr => rr.Room)
                        .ThenInclude(room => room.Facility)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<ReservationDataDto>> GetReservationsWithFacilityAsync(CancellationToken cancellationToken = default)
        {
            var results = await context.Reservations
                .Select(r => new
                {
                    Reservation = r,
                    // Get facility from packages
                    PackageFacility = r.ReservedPackages
                        .Select(rp => new
                        {
                            FacilityId = rp.Package.FacilityID,
                        })
                        .FirstOrDefault(),
                    // Get facility from rooms
                    RoomFacility = r.ReservedRooms
                        .Select(rr => new
                        {
                            FacilityId = rr.Room.FacilityID,
                        })
                        .FirstOrDefault()
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Map to DTOs
            return results.Select(r => new ReservationDataDto
            {
                ReservationId = r.Reservation.ReservationID,
                StartDate = r.Reservation.StartDate,
                EndDate = r.Reservation.EndDate,
                CreatedDate = r.Reservation.CreatedDate,
                Total = r.Reservation.Total,
                Status = r.Reservation.Status.ToString(),
                UserType = r.Reservation.UserType,
                FacilityId = r.PackageFacility?.FacilityId ?? r.RoomFacility?.FacilityId ?? 0,
            }).ToList();
        }

        public async Task<ReservationStatsDto> GetReservationStatsForLast30DaysAsync(CancellationToken cancellationToken = default)
        {
            // Calculate date range - last 30 days
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30);

            // Define the status groups
            var pendingStatuses = new[]
            {
                ReservationStatus.PendingApproval,
                ReservationStatus.PendingPayment,
                ReservationStatus.PendingPaymentVerification,
                ReservationStatus.PendingCashPayment
            };

            var cancelledOrExpiredStatuses = new[]
            {
                ReservationStatus.Cancelled,
                ReservationStatus.Expired
            };

            // Get relevant reservations for the period
            var query = context.Reservations
                .Where(r => r.EndDate >= startDate && r.StartDate <= endDate);

            // Group by status and count
            var statusGroups = await query
                .GroupBy(r => r.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            // Calculate total revenue from completed reservations
            var totalRevenue = await query
                .Where(r => r.Status == ReservationStatus.Completed)
                .SumAsync(r => r.Total, cancellationToken);

            // Build the statistics DTO
            var stats = new ReservationStatsDto
            {
                // Count pending reservations
                TotalPendingReservations = statusGroups
                    .Where(g => pendingStatuses.Contains(g.Status))
                    .Sum(g => g.Count),

                // Count completed reservations
                TotalCompletedReservations = statusGroups
                    .FirstOrDefault(g => g.Status == ReservationStatus.Completed)?.Count ?? 0,

                // Count cancelled or expired reservations
                TotalCancelledOrExpiredReservations = statusGroups
                    .Where(g => cancelledOrExpiredStatuses.Contains(g.Status))
                    .Sum(g => g.Count),

                // Total revenue
                TotalRevenue = totalRevenue
            };

            return stats;
        }

        public async Task<DailyReservationCountsResponse> GetDailyReservationCountsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            // Default to last 30 days
            var end = endDate?.Date ?? DateTime.UtcNow.Date;
            var start = startDate?.Date ?? end.AddDays(-29);

            var reservationCounts = await context.Reservations
                .Where(r => r.CreatedDate >= start && r.CreatedDate <= end.AddDays(1).AddSeconds(-1))
                .GroupBy(r => r.CreatedDate.Date)
                .Select(g => new DailyReservationCountDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var allDates = Enumerable.Range(0, (end - start).Days + 1)
                .Select(offset => start.AddDays(offset).Date)
                .ToList();

            var result = new DailyReservationCountsResponse
            {
                DailyCounts = allDates
                    .Select(date => reservationCounts.FirstOrDefault(r => r.Date.Date == date) ??
                        new DailyReservationCountDto { Date = date, Count = 0 })
                    .OrderBy(r => r.Date)
                    .ToList()
            };

            return result;
        }
    }
}
