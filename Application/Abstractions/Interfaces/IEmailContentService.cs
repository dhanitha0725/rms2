namespace Application.Abstractions.Interfaces
{
    public interface IEmailContentService 
    {
        Task<string> GeneratePendingApprovalEmailBodyAsync(
            int reservationId,
            CancellationToken cancellationToken);
    }
}
