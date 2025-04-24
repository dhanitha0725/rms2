namespace Utilities.PaymentGateway
{
    public class PayhereSettings
    {
        public string MerchantId { get; set; }
        public string MerchantSecret { get; set; }
        public string BaseUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
