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
            var amountFormatted = request.Amount.ToString("F2");

            var hash = ComputeMD5(
                _settings.MerchantId +
                request.OrderId +
                amountFormatted +
                request.Currency +
                hashedSecret
            );

            return new PaymentInitiationResponse
            {
                MerchantId = _settings.MerchantId,
                ReturnUrl = _settings.ReturnUrl,
                CancelUrl = _settings.CancelUrl,
                NotifyUrl = _settings.NotifyUrl,
                Hash = hash,
                // Other parameters from request
            };
        }
        public bool VerifyWebhook(WebhookNotification notification)
        {
            var hashedSecret = ComputeMD5(_settings.MerchantSecret);
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
