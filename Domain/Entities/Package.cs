namespace Domain.Entities
{
    public class Package
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public TimeSpan? Duration { get; set; }
        public int FacilityID { get; set; }

        public Facility Facility { get; set; }
        public ICollection<ReservedPackage> ReservedPackages { get; set; }
        public ICollection<Pricing> Pricings { get; set; }
    }
}
