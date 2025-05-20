namespace Application.DTOs.InvoiceDtos
{
    public class InvoiceTableDataDto
    {
        public int InvoiceID { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime IssuedDate { get; set; }
        public int ReservationID { get; set; }
        public Guid? PaymentID { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class InvoiceDetailsDto
    {
        public int InvoiceId { get; set; }
        public Guid? PaymentId { get; set; }
        public int ReservationId { get; set; }
        public string FacilityName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string ReservationStatus { get; set; }
        public DateTime ReservationStartDate { get; set; }
        public DateTime ReservationEndDate { get; set; }
    }


}