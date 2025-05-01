using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using Microsoft.Extensions.Options;

namespace Utilities.PaymentGateway
{
    public class PayhereService : IPayhereService
    {
        private readonly PayhereSettings _settings;

        public PayhereService(IOptions<PayhereSettings> settings)
        {
            _settings = settings.Value;
        }

        public PaymentInitiationResponse PrepareCheckout(PaymentRequest request)
        {
            var hashedSecret = ComputeMD5(_settings.MerchantSecret);
            var amountFormatted = request.Amount.ToString("F2", CultureInfo.InvariantCulture);

            var hash = ComputeMD5(
                _settings.MerchantId +
                request.OrderId +
                amountFormatted +
                request.Currency +
                hashedSecret
            );

            return new PaymentInitiationResponse
            {
                ActionUrl = _settings.BaseUrl,       // "https://sandbox.payhere.lk/pay/checkout"
                MerchantId = _settings.MerchantId,   // From settings
                ReturnUrl = _settings.ReturnUrl,     // "http://localhost:5173/payment/status"
                CancelUrl = _settings.CancelUrl,     // "http://localhost:5173/payment/cancel"
                NotifyUrl = _settings.NotifyUrl,     // "https://d6f3-192-248-24-51.ngrok-free.app/"
                Hash = hash,
                FirstName = request.FirstName,       
                LastName = request.LastName,         
                Email = request.Email,               
                Phone = request.Phone,               
                Address = request.Address,           
                City = request.City,                 
                Country = request.Country,           
                Items = request.Items,               
                OrderId = request.OrderId,           
                Currency = request.Currency,         
                Amount = request.Amount,             
                AmountFormatted = amountFormatted    
            };
        }
        public bool VerifyWebhook(WebhookNotification notification)
        {
            // hash the merchant secret
            var hashedSecret = ComputeMD5(_settings.MerchantSecret);

       
            // compute the local hash
            var localHash = ComputeMD5(
                notification.MerchantId +
                notification.OrderId +
                notification.PayhereAmount + 
                notification.PayhereCurrency +
                notification.StatusCode +
                hashedSecret
            );

            return localHash.Equals(notification.Md5Sig, StringComparison.OrdinalIgnoreCase);
        }

        private static string ComputeMD5(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
