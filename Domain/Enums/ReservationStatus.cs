namespace Domain.Enums
{
    public enum ReservationStatus
    {
        PendingDocumentUpload,
        PendingApproval,
        PaymentPending,
        Confirmed,
        Cancelled,
        Expired
    }
}
