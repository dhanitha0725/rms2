namespace Application.DTOs.FacilityDtos
{
    public class RoomConfigurationDto
    {
        public string RoomType { get; set;}
        public int Quantity { get; set; }
        public int Capacity { get; set; }
        public int NumberOfBeds { get; set; }
        public string Status { get; set; } = "Available";
    }
}
