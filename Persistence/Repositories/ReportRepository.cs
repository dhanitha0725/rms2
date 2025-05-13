using Application.Abstractions.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;

namespace Persistence.Repositories
{
    public class ReportRepository(ReservationDbContext context) : IReportRepository
    {

        public async Task<List<FinancialReport>> GetFinancialReportAsync(
            DateTime startDate,
            DateTime endDate,
            int? facilityId,
            CancellationToken cancellationToken = default)
        {
            var sql = @"
                SELECT
                    f.""FacilityID"" AS ""FacilityId"",
                    f.""FacilityName"",
                    COUNT(DISTINCT res.""ReservationID"") AS ""TotalReservations"",
                    COALESCE(SUM(res.""TotalPaid""), 0) AS ""TotalRevenue""
                FROM ""Facilities"" f
                LEFT JOIN (
                    -- Room-based reservations
                    SELECT
                        r.""ReservationID"",
                        rm.""FacilityID"",
                        (r.""EndDate"" AT TIME ZONE 'Asia/Kolkata')::date AS ""EndDate"",
                        p.""TotalPaid""
                    FROM ""Reservations"" r
                    JOIN ""ReservedRooms"" rr ON rr.""ReservationID"" = r.""ReservationID""
                    JOIN ""Rooms"" rm ON rm.""RoomID"" = rr.""RoomID""
                    LEFT JOIN (
                        SELECT ""ReservationID"", SUM(""AmountPaid"") AS ""TotalPaid""
                        FROM ""Payments""
                        WHERE ""Status"" = 'Completed'
                        GROUP BY ""ReservationID""
                    ) p ON p.""ReservationID"" = r.""ReservationID""
                    WHERE r.""Status"" IN ('Confirmed', 'Completed')
                      AND (r.""EndDate"" AT TIME ZONE 'Asia/Kolkata')::date BETWEEN @startDate AND @endDate

                    UNION ALL

                    -- Package-based reservations
                    SELECT
                        r.""ReservationID"",
                        pkg.""FacilityID"",
                        (r.""EndDate"" AT TIME ZONE 'Asia/Kolkata')::date AS ""EndDate"",
                        p.""TotalPaid""
                    FROM ""Reservations"" r
                    JOIN ""ReservedPackages"" rp ON rp.""ReservationID"" = r.""ReservationID""
                    JOIN ""Packages"" pkg ON pkg.""PackageID"" = rp.""PackageID""
                    LEFT JOIN (
                        SELECT ""ReservationID"", SUM(""AmountPaid"") AS ""TotalPaid""
                        FROM ""Payments""
                        WHERE ""Status"" = 'Completed'
                        GROUP BY ""ReservationID""
                    ) p ON p.""ReservationID"" = r.""ReservationID""
                    WHERE r.""Status"" IN ('Confirmed', 'Completed')
                      AND (r.""EndDate"" AT TIME ZONE 'Asia/Kolkata')::date BETWEEN @startDate AND @endDate
                ) res ON res.""FacilityID"" = f.""FacilityID""
                WHERE (@facilityId IS NULL OR f.""FacilityID"" = @facilityId::integer)
                GROUP BY f.""FacilityID"", f.""FacilityName""
                ORDER BY f.""FacilityName"";
            ";

            var result = await context.Set<FinancialReport>()
                .FromSqlRaw(sql,
                    new Npgsql.NpgsqlParameter("startDate", startDate),
                    new Npgsql.NpgsqlParameter("endDate", endDate),
                    new Npgsql.NpgsqlParameter("facilityId", System.Data.DbType.Int32) 
                        { Value = (object?)facilityId ?? DBNull.Value }
                )
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
