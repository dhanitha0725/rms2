namespace Domain.Entities
{
    public class Facility
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        public FacilityType FacilityType { get; set; }
        public List<string>? Attributes { get; set; }
        public string Location { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedDate { get; set; }

        // new properties for sub facilities
        public int? ParentFacilityId { get; set; }
        public Facility? ParentFacility { get; set; }
        public ICollection<Facility>? ChildFacilities { get; set; }

        public ICollection<Package> Packages{ get; set; }
        public ICollection<Room>? Rooms { get; set; }
        public ICollection<RoomPricing>? RoomPricings { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}