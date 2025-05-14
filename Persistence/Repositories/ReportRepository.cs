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
            IEnumerable<int>? facilityIds,
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
                WHERE (
                    @facilityIds IS NULL OR
                    f.""FacilityID"" = ANY(@facilityIds)
                )
                GROUP BY f.""FacilityID"", f.""FacilityName""
                ORDER BY f.""FacilityName"";
            ";

            var facilityIdsParam = new Npgsql.NpgsqlParameter(
                "facilityIds", NpgsqlTypes.NpgsqlDbType.Integer | NpgsqlTypes.NpgsqlDbType.Array)
            {
                Value = (facilityIds != null && facilityIds.Any()) 
                    ? facilityIds.ToArray() : DBNull.Value
            };

            var result = await context.Set<FinancialReport>()
                .FromSqlRaw(sql,
                    new Npgsql.NpgsqlParameter("startDate", startDate),
                    new Npgsql.NpgsqlParameter("endDate", endDate),
                    facilityIdsParam
                )
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
