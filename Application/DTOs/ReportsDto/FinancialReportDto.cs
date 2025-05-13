namespace Application.DTOs.ReportsDto
{
    public class FinancialReportDto
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int TotalReservations { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
