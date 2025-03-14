namespace Domain.Entities
{
    public class Payment
    {
        public Guid PaymentID { get; set; }
        public string Method { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public int ReservationID { get; set; }
        public int UserID { get; set; }

        public Reservation Reservation { get; set; }
        public User User { get; set; }
        public ICollection<InvoicePayment> InvoicePayments { get; set; }
    }
}