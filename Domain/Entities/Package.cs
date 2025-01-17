namespace Domain.Entities
{
    public class Package
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<FacilityPackage> FacilityPackages { get; set; }
        public ICollection<ReservedPackage> ReservedPackages { get; set; }
        public ICollection<Pricing> Pricings { get; set; }
    }
}
