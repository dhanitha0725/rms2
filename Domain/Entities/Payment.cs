namespace Domain.Entities
{
    public class Payment
    {
        public Guid PaymentID { get; set; }
        public string? OrderID { get; set; }
        public string Method { get; set; }
        public decimal? AmountPaid { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string? GatewayTransactionID { get; set; }
        public int ReservationID { get; set; }
        public int ReservationUserID { get; set; }

        public Reservation Reservation { get; set; }
        public ReservationUserDetail User { get; set; }
        public ICollection<InvoicePayment> InvoicePayments { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}