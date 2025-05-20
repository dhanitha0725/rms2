using System.Linq.Expressions;
using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Entities;
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
    }
}
