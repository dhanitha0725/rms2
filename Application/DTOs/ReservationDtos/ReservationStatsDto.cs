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
}
