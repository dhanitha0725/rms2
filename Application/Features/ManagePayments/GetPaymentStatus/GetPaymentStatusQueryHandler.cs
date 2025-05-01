using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManagePayments.GetPaymentStatus
{
    public class GetPaymentStatusQueryHandler(
            IGenericRepository<Payment, Guid> paymentRepository,
            ILogger logger) : IRequestHandler<GetPaymentStatusQuery, Result<PaymentStatusDto>>
    {
        public async Task<Result<PaymentStatusDto>> Handle(
            GetPaymentStatusQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                // Fetch the payment record by OrderId
                var payment = (await paymentRepository.GetAllAsync(cancellationToken))
                    .FirstOrDefault(p => p.OrderID == request.OrderId);

                if (payment == null)
                {
                    logger.Warning("Payment not found for OrderId: {OrderId}", request.OrderId);
                    return Result<PaymentStatusDto>.Failure(new Error("Payment not found."));
                }

                // Map the payment details to the DTO
                var paymentStatusDto = new PaymentStatusDto
                {
                    ReservationId = payment.ReservationID,
                    OrderId = payment.OrderID,
                    Status = payment.Status,
                    Amount = payment.AmountPaid ?? 0,
                    Message = payment.Status switch
                    {
                        "Completed" => "Payment successful",
                        "Failed" => "Payment failed",
                        _ => "Payment is pending"
                    }
                };

                return Result<PaymentStatusDto>.Success(paymentStatusDto);
            }
            catch (Exception ex)
            {
                logger.Error("Error fetching payment status: {Message}", ex.Message);
                return Result<PaymentStatusDto>.Failure(new Error("An error occurred while fetching payment status."));
            }
        }
    }
}
