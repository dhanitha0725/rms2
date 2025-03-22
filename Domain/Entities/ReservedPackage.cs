namespace Domain.Entities
{
    public class ReservedPackage
    {
        public int ReservedPackageID { get; set; }
        public int PackageID { get; set; }
        public int ReservationID { get; set; }
        public string status { get; set; }

        public Package Package { get; set; }
        public Reservation Reservation { get; set; }
    }
}