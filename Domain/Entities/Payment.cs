namespace Domain.Entities
{
    public class Payment
    {
        public Guid PaymentID { get; set; }
        public string Method { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public Guid ReservationID { get; set; }
        public int UserID { get; set; }
        public int InvoiceID { get; set; }

        public Reservation Reservation { get; set; }
        public User User { get; set; }
        public Invoice Invoice { get; set; }
    }
}