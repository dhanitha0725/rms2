namespace Domain.Entities
{
    public class Facility
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        public FacilityType FacilityType { get; set; }
        public string? Attributes { get; set; }
        public string Location { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool HasRooms { get; set; }
        public bool HasPackages { get; set; }

        public ICollection<Package> Packages{ get; set; }
        public ICollection<Room>? Rooms { get; set; }
        public ICollection<RoomPricing>? RoomPricings { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}