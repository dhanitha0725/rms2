using Application.DTOs.Payment;

namespace Application.Abstractions.Interfaces
{
    public interface IPayhereService
    {
        PaymentInitiationResponse PrepareCheckout(PaymentRequest request);
        bool VerifyWebhook(WebhookNotification notification);
    }
}
