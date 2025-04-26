namespace Domain.Entities
{
    public class Document
    {
        public int DocumentID { get; set; }
        public string DocumentType { get; set; }
        public string Url { get; set; }
        public int? ReservationId { get; set; }
        public Guid? PaymentId { get; set; }

        public Reservation Reservation { get; set; }
        public Payment Payment { get; set; }
    }
}
