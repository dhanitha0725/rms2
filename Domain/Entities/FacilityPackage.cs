namespace Domain.Entities
{
    public class FacilityPackage
    {
        public Guid FacilityID { get; set; }
        public int PackageID { get; set; }

        public Facility Facility { get; set; }
        public Package Package { get; set; }
    }
}