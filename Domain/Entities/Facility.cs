namespace Domain.Entities
{
    public class Facility
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        public FacilityType FacilityType { get; set; }
        public string? Attributes { get; set; } // JSON type
        public string Location { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedDate { get; set; } 

        public ICollection<FacilityPackage> FacilityPackages { get; set; }
        public ICollection<Room>? Rooms { get; set; }
    }
}