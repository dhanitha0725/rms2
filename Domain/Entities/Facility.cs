namespace Domain.Entities
{
    public class Facility
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public string? Attributes { get; set; } // JSON column
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; } 
        public DateTime? UpdatedDate { get; set; }

        public ICollection<FacilityPackage> FacilityPackages { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}