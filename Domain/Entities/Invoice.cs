namespace Domain.Entities
{
    public class Invoice
    {
        public int InvoiceID { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime IssuedDate { get; set; }
        public Guid ReservationID { get; set; }

        public Reservation Reservation { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}