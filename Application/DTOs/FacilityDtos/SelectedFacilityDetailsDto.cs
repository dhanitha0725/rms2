namespace Application.DTOs.FacilityDtos
{
    public class SelectedFacilityDetailsDto
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string Location { get; set; }
        public string? Description { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<PackageDetailsDto> Packages { get; set; } 
        public List<RoomDetailsDto> Rooms { get; set; } 
    }

    public class PackageDetailsDto
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public TimeSpan? Duration { get; set; }
        public List<PricingDto> Pricing { get; set; }
    }

    public class PricingDto
    {
        public string Sector { get; set; }
        public decimal Price { get; set; }
    }

    public class RoomDetailsDto
    {
        public int RoomTypeId { get; set; }
        public string RoomType { get; set; }
        public List<RoomPricingDto> RoomPricing { get; set; }
    }

    public class RoomPricingDto
    {
        public string Sector { get; set; }
        public decimal Price { get; set; }
    }
}
        