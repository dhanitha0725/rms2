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
        public string AmountFormatted { get; set; }
    }

    public class WebhookNotification
    {
        // Standard PayHere webhook parameters  
        public string merchant_id { get; set; }
        public string order_id { get; set; }
        public string payment_id { get; set; }
        public string payhere_amount { get; set; }
        public string payhere_currency { get; set; }
        public int status_code { get; set; }
        public string md5sig { get; set; }
        public string method { get; set; }
        public string status_message { get; set; }

        // Map to your command properties  
        public string MerchantId => merchant_id;
        public string OrderId => order_id;
        public string PaymentId => payment_id;
        public decimal PayhereAmount => decimal.TryParse(payhere_amount, out var amount) ? amount : 0;
        public string PayhereCurrency => payhere_currency;
        public int StatusCode => status_code;
        public string Md5Sig => md5sig;

        // Parameterless constructor for model binding  
        public WebhookNotification()
        {
        }

        // Updated constructor to initialize fields directly  
        public WebhookNotification(
            string merchant_id,
            string order_id,
            string payment_id,
            string payhere_amount,
            string payhere_currency,
            int status_code,
            string md5sig)
        {
            this.merchant_id = merchant_id;
            this.order_id = order_id;
            this.payment_id = payment_id;
            this.payhere_amount = payhere_amount;
            this.payhere_currency = payhere_currency;
            this.status_code = status_code;
            this.md5sig = md5sig;
        }
    }
}
