using Domain.Entities;

namespace Application.DTOs.PackageDto
{
    public class PackageTableDetailsDto
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public TimeSpan? Duration { get; set; }
        public string FacilityName { get; set; }
        public Dictionary<string, decimal> Pricings { get; set; } = new Dictionary<string, decimal>();
    }

    public class RoomPricingTableDto
    {
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public string FacilityName { get; set; }
        public int TotalRooms { get; set; }
        public Dictionary<string, decimal> Pricings { get; set; } = new Dictionary<string, decimal>();
    }
}
