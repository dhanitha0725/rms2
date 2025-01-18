namespace Domain.Entities
{
    public class FacilityPackage
    {
        public int FacilityID { get; set; }
        public int PackageID { get; set; }

        public Facility Facility { get; set; }
        public Package Package { get; set; }
    }
}