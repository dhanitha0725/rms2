namespace Application.DTOs.Payment
{
    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "LKR";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; } = "Sri Lanka";
        public string Items { get; set; }
    }

    public class PaymentInitiationResponse
    {
        public string ActionUrl { get; set; }
        public string MerchantId { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string Hash { get; set; }
    }

    public class WebhookNotification
    {
        public string MerchantId { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string PayhereAmount { get; set; }
        public string PayhereCurrency { get; set; }
        public int StatusCode { get; set; }
        public string Md5Sig { get; set; }
        // Other parameters
    }
}
