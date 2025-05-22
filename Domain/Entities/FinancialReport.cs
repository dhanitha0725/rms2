namespace Domain.Entities
{
    public class FinancialReport
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int TotalReservations { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
