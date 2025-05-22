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

        Task<string> GenerateBankTransferInstructionsEmailBodyAsync(
            int reservationId,
            CancellationToken cancellationToken);

        Task<string> GenerateCashPaymentInstructionsEmailBodyAsync(
            int reservationId,
            CancellationToken cancellationToken);

        Task<string> GeneratePaymentEmailAsync(
            int reservationId,
            string email,
            string paymentLink,
            CancellationToken cancellationToken);

        Task<string> GenerateReservationCancellationEmailAsync(
            int reservationId,
            string email,
            CancellationToken cancellationToken);
    }
}
