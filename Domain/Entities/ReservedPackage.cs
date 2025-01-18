namespace Domain.Entities
{
    public class ReservedPackage
    {
        public int ReservedPackageID { get; set; }
        public int PackageID { get; set; }
        public int ReservationID { get; set; }
        public int Quantity { get; set; }

        public Package Package { get; set; }
        public Reservation Reservation { get; set; }
    }
}