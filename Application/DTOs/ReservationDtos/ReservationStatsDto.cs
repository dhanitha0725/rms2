using System;

namespace Application.DTOs.ReservationDtos
{
    public class ReservationStatsDto
    {
        public int TotalPendingReservations { get; set; }
        public int TotalCompletedReservations { get; set; }
        public int TotalCancelledOrExpiredReservations { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DailyReservationCountDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class DailyReservationCountsResponse
    {
        public List<DailyReservationCountDto> DailyCounts { get; set; } = new List<DailyReservationCountDto>();
    }
}
