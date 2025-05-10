namespace Domain.Enums
{
    public enum ReservationStatus
    {
        PendingApproval,
        PendingPayment,
        PendingCashPayment,
        PendingPaymentVerification,
        Approved,
        Completed,
        Cancelled,
        Confirmed,
        Expired
    }
}
