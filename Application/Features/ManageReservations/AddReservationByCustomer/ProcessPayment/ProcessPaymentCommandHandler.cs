using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.AddReservationByCustomer.ProcessPayment
{
    public class ProcessPaymentCommandHandler(
            IGenericRepository<Reservation, int> reservationRepository,
            IGenericRepository<Payment, Guid> paymentRepository,
            IGenericRepository<User, int> userRepository,
            IGenericRepository<Document, int> documentRepository,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IGoogleDriveService googleDriveService,
            ILogger logger)
            : IRequestHandler<ProcessPaymentCommand, Result>
    {
        public async Task<Result> Handle(
            ProcessPaymentCommand request,
            CancellationToken cancellationToken)
        {
            // check if reservation exists
            var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
            if (reservation == null)
            {
                return Result.Failure(new Error("Reservation not found"));
            }

            // Ensure payments collection is initialized
            var reservationPayments = reservation.Payments ?? new List<Payment>();

            // Create payment record
            var payment = new Payment
            {
                PaymentID = Guid.NewGuid(),
                Method = request.PaymentMethod,
                AmountPaid = request.PaymentMethod == "GoAndPay" ? 0 : request.AmountPaid,
                CreatedDate = DateTime.UtcNow,
                Status = "Pending",
                ReservationID = request.ReservationId,
                UserID = request.UserId,
            };

            // Process payment based on payment method
            switch (request.PaymentMethod)
            {
                case "GoAndPay":
                    payment.Status = "Pending";
                    break;

                case "BankTransfer":
                    if (request.BankTransferReceipts == null || !request.BankTransferReceipts.Any())
                    {
                        return Result.Failure(new Error("Bank transfer receipt is required"));
                    }

                    try
                    {
                        // upload documents
                        var uploadedUrls = await googleDriveService.UploadFilesAsync(request.BankTransferReceipts);

                        var documents = uploadedUrls.Select(url => new Document
                        {
                            DocumentType = "BankTransferReceipt",
                            Url = url,
                            ReservationID = request.ReservationId,
                            UserID = request.UserId
                        }).ToList();

                        await documentRepository.AddRangeAsync(documents, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Error uploading bank transfer receipt");
                        return Result.Failure(new Error("Error uploading bank transfer receipt"));
                    }

                    payment.Status = "Pending Verification";
                    break;

                case "Online":
                    payment.Status = "Paid";
                    payment.AmountPaid = request.AmountPaid;
                    break;

                default:
                    return Result.Failure(new Error("Invalid payment method"));
            }

            await paymentRepository.AddAsync(payment, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            decimal totalPaid = reservationPayments.Sum(p => p.AmountPaid);

            // Update reservation status based on total paid amount
            if (totalPaid >= reservation.Total)
            {
                reservation.Status = "Confirmed";
            }
            else if (totalPaid > 0)
            {
                reservation.Status = "Partially Paid";
            }
            else
            {
                reservation.Status = request.PaymentMethod == "BankTransfer"
                    ? "Pending Payment Verification"
                    : "Pending Payment";
            }

            await reservationRepository.UpdateAsync(reservation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // send email
            if (reservation.Status == "Confirmed")
            {
                var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user != null)
                {
                    string emailSubject = "Reservation Confirmation";
                    string emailBody = $@"
                            <html>
                            <body>
                                <h2>Reservation Confirmation</h2>
                                <p>Dear {user.FirstName} {user.LastName},</p>
                                <p>Your reservation (ID: {request.ReservationId}) has been confirmed.</p>
                                <p>Thank you for your payment.</p>
                                <p>Reservation Details:</p>
                                <ul>
                                    <li>Start Date: {reservation.StartDate:yyyy-MM-dd}</li>
                                    <li>End Date: {reservation.EndDate:yyyy-MM-dd}</li>
                                    <li>Total Amount: {reservation.Total}</li>
                                </ul>
                                <p>Best regards,<br>The Reservation Team</p>
                            </body>
                            </html>";

                    await emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
                    logger.Information("Confirmation email sent to user {UserId}", user.UserId);
                }
            }

            return Result.Success([]);
        }
    }
}
