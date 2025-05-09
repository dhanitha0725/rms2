namespace Application.DTOs.ReservationDtos
{
    public class CheckAvailabilityDto
    {
        public int FacilityId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<AvailableItemDto> Items { get; set; } = new();
    }

    public class AvailableItemDto
    {
        public int ItemId { get; set; }
        public string Type { get; set; } //package or room
        public int Quantity { get; set; } = 1;
    }

    public class AvailabilityResponseDto
    {
        public bool IsAvailable { get; set; }
        public string Message { get; set; }
    }

}
