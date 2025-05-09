using Domain.Enums;

namespace Domain.Entities
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public decimal Total { get; set; }
        public ReservationStatus Status { get; set; }
        public string UserType { get; set; }

        public ICollection<Payment>? Payments { get; set; }
        public ICollection<ReservedPackage> ReservedPackages { get; set; }
        public ICollection<ReservedRoom>? ReservedRooms { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
        public ICollection<Document>? Documents { get; set; }
        public ReservationUserDetail ReservationUserDetail { get; set; }
    }
}
