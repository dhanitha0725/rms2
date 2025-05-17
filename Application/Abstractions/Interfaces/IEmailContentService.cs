namespace Application.Abstractions.Interfaces
{
    public interface IEmailContentService
    {
        Task<string> GeneratePendingApprovalEmailBodyAsync(
            int reservationId,
            CancellationToken cancellationToken);

        Task<string> GeneratePaymentConfirmationEmailBodyAsync(
            int reservationId,
            CancellationToken cancellationToken);
    }
}
