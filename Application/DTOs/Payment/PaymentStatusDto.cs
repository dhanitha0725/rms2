namespace Application.DTOs.Payment
{
    public class PaymentStatusDto
    {
        public int ReservationId { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
    }
}
