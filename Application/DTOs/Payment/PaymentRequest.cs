namespace Application.DTOs.Payment
{
    public class PaymentRequest
    {
        public PaymentRequest(
            string orderId,
            decimal amount,
            string currency,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string city,
            string items)
        {
            OrderId = orderId;
            Amount = amount;
            Currency = currency;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Address = address;
            City = city;
            Items = items;
            Country = "Sri Lanka";
        }

        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Items { get; set; }
    }

    public class PaymentInitiationResponse
    {
        // Add all required form fields
        public string ActionUrl { get; set; }
        public string MerchantId { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string Hash { get; set; }

        // Add customer details
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Items { get; set; }
        public string OrderId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }

    public class WebhookNotification
    {
        public WebhookNotification(
            string merchantId,
            string orderId,
            string paymentId,
            string payhereAmount,
            string payhereCurrency,
            int statusCode,
            string md5Sig)
        {
            MerchantId = merchantId;
            OrderId = orderId;
            PaymentId = paymentId;
            PayhereAmount = payhereAmount;
            PayhereCurrency = payhereCurrency;
            StatusCode = statusCode;
            Md5Sig = md5Sig;
        }

        public string MerchantId { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string PayhereAmount { get; set; }
        public string PayhereCurrency { get; set; }
        public int StatusCode { get; set; }
        public string Md5Sig { get; set; }
    }
}
