namespace Application.DTOs.ReservationDtos
{
    public class CalculateTotalDto
    {
        public int FacilityId { get; set; }
        public string CustomerType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<BookingItemDto> SelectedItems { get; set; } = new ();
    }

    public class BookingItemDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; } // package or room
    }

    public class CalculateTotalResponseDto
    {
        public decimal Total { get; set; }
        public List<PriceBreakdownDto> Breakdown { get; set; } = new();
    }

    public class PriceBreakdownDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string PricingType { get; set; } = string.Empty; // fixed or per day
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
