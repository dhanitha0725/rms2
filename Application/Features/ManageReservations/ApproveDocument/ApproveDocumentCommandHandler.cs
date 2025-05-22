using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application.Features.ManageReservations.ApproveDocument
{
    public class ApproveDocumentCommandHandler(
            IGenericRepository<Reservation, int> reservationRepository,
            IGenericRepository<Payment, Guid> paymentRepository,
            IGenericRepository<Document, int> documentRepository,
            IGenericRepository<Room, int> roomRepository,
            IGenericRepository<ReservationUserDetail, int> reservationUserRepository,
            IGenericRepository<ReservedRoom, int> reservedRoomRepository,
            IGenericRepository<Invoice, int> invoiceRepository,
            IGenericRepository<InvoicePayment, object> invoicePaymentRepository,
            IUnitOfWork unitOfWork,
            IBackgroundTaskQueue backgroundTaskQueue,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration,
            ILogger logger)
            : IRequestHandler<ApproveDocumentCommand, Result<PaymentInitiationResponse>>
    {
        public async Task<Result<PaymentInitiationResponse>> Handle(
            ApproveDocumentCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Retrieve the document
                var document = await documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
                if (document == null)
                    return Result<PaymentInitiationResponse>.Failure(new Error("Document Not Found"));

                if (request.IsApproved)
                {
                    // Handle ApprovalDocument
                    if (request.DocumentType == nameof(DocumentType.ApprovalDocument))
                    {
                        if (document.ReservationId == null)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Document is not associated with a reservation."));

                        var reservation = await reservationRepository.GetByIdAsync(document.ReservationId.Value, cancellationToken);
                        if (reservation == null)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Reservation not found"));

                        if (reservation.Status != ReservationStatus.PendingApproval)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Reservation is not in PendingApproval status."));

                        // Update reservation status to PendingPayment
                        reservation.Status = ReservationStatus.PendingPayment;
                        reservation.UpdatedDate = DateTime.UtcNow;
                        await reservationRepository.UpdateAsync(reservation, cancellationToken);
                        logger.Information("Reservation {ReservationId} status updated to PendingPayment after document approval.", reservation.ReservationID);

                        // receive reservation user details
                        var reservationUserDetail = await reservationUserRepository.GetByIdAsync(
                            (await reservationUserRepository.GetAllAsync(cancellationToken))
                            .FirstOrDefault(ud => ud.ReservationID == reservation.ReservationID)?.ReservationUserDetailID ?? 0,
                            cancellationToken);

                        if (reservationUserDetail == null)
                        {
                            return Result<PaymentInitiationResponse>.Failure(new Error("Reservation user details not found"));
                        }

                        // create payment record
                        var payment = new Payment
                        {
                            OrderID = request.OrderId,
                            AmountPaid = reservation.Total > 0 ? request.Amount : reservation.Total,
                            Currency = "LKR",
                            CreatedDate = DateTime.UtcNow,
                            Status = "Pending",
                            Method = nameof(PaymentMethods.Online),
                            ReservationID = reservation.ReservationID,
                            ReservationUserID = reservationUserDetail.ReservationUserDetailID,
                        };

                        await paymentRepository.AddAsync(payment, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);

                        // generate payment link
                        var baseUrl = configuration["Application:FrontendBaseUrl"];
                        var paymentLink = $"{baseUrl}/payment-initiate?orderId={Uri.EscapeDataString(request.OrderId)}";

                        // schedule email
                        SchedulePaymentEmail(reservation.ReservationID, reservationUserDetail.Email, paymentLink);
                    }
                    // Handle BankReceipt
                    else if (request.DocumentType == nameof(DocumentType.BankReceipt))
                    {
                        if (document.PaymentId == null)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Document is not associated with a payment."));

                        var payment = await paymentRepository.GetByIdAsync(document.PaymentId.Value, cancellationToken);
                        if (payment == null)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Payment not found."));

                        var reservation = await reservationRepository.GetByIdAsync(payment.ReservationID, cancellationToken);
                        if (reservation == null)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Reservation not found."));

                        if (reservation.Status != ReservationStatus.PendingPaymentVerification)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Reservation is not in PendingPaymentVerification status."));

                        if (request.AmountPaid == null)
                        {
                            return Result<PaymentInitiationResponse>.Failure(new Error("Amount paid is required when approving bank receipt."));
                        }

                        payment.AmountPaid = request.AmountPaid.Value;
                        payment.Status = "Completed";
                        await paymentRepository.UpdateAsync(payment, cancellationToken);
                        logger.Information("Payment {PaymentId} status updated to Completed after document approval.", payment.PaymentID);

                        reservation.Status = ReservationStatus.Confirmed;
                        reservation.UpdatedDate = DateTime.UtcNow;
                        await reservationRepository.UpdateAsync(reservation, cancellationToken);
                        logger.Information("Reservation {ReservationId} status updated to Confirmed after payment completion.", reservation.ReservationID);

                        // Create invoice when reservation is confirmed
                        var invoice = new Invoice
                        {
                            ReservationID = reservation.ReservationID,
                            AmountDue = reservation.Total - payment.AmountPaid ?? 0,
                            AmountPaid = payment.AmountPaid ?? 0,
                            IssuedDate = DateTime.UtcNow
                        };

                        await invoiceRepository.AddAsync(invoice, cancellationToken);
                        logger.Information("Invoice created for reservation {ReservationId}", reservation.ReservationID);

                        // Link payment to invoice
                        await unitOfWork.SaveChangesAsync(cancellationToken); // Save to get the invoice ID

                        var invoicePayment = new InvoicePayment
                        {
                            InvoiceID = invoice.InvoiceID,
                            PaymentID = payment.PaymentID
                        };

                        await invoicePaymentRepository.AddAsync(invoicePayment, cancellationToken);
                        logger.Information("Payment {PaymentId} linked to invoice {InvoiceId}", payment.PaymentID, invoice.InvoiceID);

                        // Add confirmation email scheduling here if needed
                        SchedulePaymentConfirmationEmail(reservation.ReservationID, payment.ReservationUserID);
                    }
                    else
                    {
                        return Result<PaymentInitiationResponse>.Failure(new Error("Invalid Document Type"));
                    }
                }
                else
                {
                    // Handle document rejection
                    if (request.DocumentType == nameof(DocumentType.BankReceipt))
                    {
                        if (document.PaymentId == null)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Document is not associated with a payment."));

                        var payment = await paymentRepository.GetByIdAsync(document.PaymentId.Value, cancellationToken);
                        if (payment == null)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Payment not found."));

                        var reservation = await reservationRepository.GetByIdAsync(payment.ReservationID, cancellationToken);
                        if (reservation == null)
                            return Result<PaymentInitiationResponse>.Failure(new Error("Reservation not found."));

                        payment.Status = "Cancelled";
                        await paymentRepository.UpdateAsync(payment, cancellationToken);

                        reservation.Status = ReservationStatus.Cancelled;
                        reservation.UpdatedDate = DateTime.UtcNow;
                        await reservationRepository.UpdateAsync(reservation, cancellationToken);
                        logger.Information("Reservation {ReservationId} and Payment {PaymentId} status updated to Cancelled after bank receipt rejection.", reservation.ReservationID, payment.PaymentID);

                        // Release rooms using the helper method
                        await ReleaseRoomsForReservationAsync(reservation.ReservationID, cancellationToken);

                        // Get user email and send cancellation notification
                        var user = await reservationUserRepository.GetByIdAsync(payment.ReservationUserID, cancellationToken);
                        if (user != null && !string.IsNullOrEmpty(user.Email))
                        {
                            ScheduleCancellationEmail(reservation.ReservationID, user.Email);
                            logger.Information("Cancellation email scheduled for reservation {ReservationId}.", reservation.ReservationID);
                        }
                    }
                    else
                    {
                        if (document.ReservationId != null)
                        {
                            var reservation = await reservationRepository.GetByIdAsync(document.ReservationId.Value, cancellationToken);
                            if (reservation != null)
                            {
                                reservation.Status = ReservationStatus.Cancelled;
                                reservation.UpdatedDate = DateTime.UtcNow;
                                await reservationRepository.UpdateAsync(reservation, cancellationToken);
                                logger.Information("Reservation {ReservationId} status updated to Cancelled after document rejection.", reservation.ReservationID);

                                // Release rooms using the helper method
                                await ReleaseRoomsForReservationAsync(reservation.ReservationID, cancellationToken);

                                // Find user associated with this reservation to send cancellation email
                                var reservationUserDetail = await reservationUserRepository.GetByIdAsync(
                                    (await reservationUserRepository.GetAllAsync(cancellationToken))
                                    .FirstOrDefault(ud => ud.ReservationID == reservation.ReservationID)?.ReservationUserDetailID ?? 0,
                                    cancellationToken);

                                if (reservationUserDetail != null && !string.IsNullOrEmpty(reservationUserDetail.Email))
                                {
                                    ScheduleCancellationEmail(reservation.ReservationID, reservationUserDetail.Email);
                                    logger.Information("Cancellation email scheduled for reservation {ReservationId}.", reservation.ReservationID);
                                }
                            }
                        }
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("{PaymentInitiationResponse}", new PaymentInitiationResponse());
                return Result<PaymentInitiationResponse>.Success(new PaymentInitiationResponse());

            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error processing document approval");
                return Result<PaymentInitiationResponse>.Failure(
                    new Error("An error occurred while processing the request."));
            }
        }

        // Helper method to release rooms for a reservation
        private async Task ReleaseRoomsForReservationAsync(int reservationId, CancellationToken cancellationToken)
        {
            // Get all reserved rooms for this reservation
            var reservedRooms = await reservedRoomRepository.GetAllAsync(
                rr => rr.ReservationID == reservationId,
                cancellationToken);

            if (reservedRooms == null || !reservedRooms.Any())
            {
                logger.Warning("No reserved rooms found for reservation {ReservationId}.", reservationId);
                return;
            }

            // Update each room status to Available
            foreach (var reservedRoom in reservedRooms)
            {
                var room = await roomRepository.GetByIdAsync(reservedRoom.RoomID, cancellationToken);
                if (room != null)
                {
                    room.Status = "Available";
                    await roomRepository.UpdateAsync(room, cancellationToken);
                    logger.Information("Room {RoomId} status updated to Available after reservation cancellation.", room.RoomID);
                }
                else
                {
                    logger.Warning("Room {RoomId} not found for reservation {ReservationId}.",
                        reservedRoom.RoomID, reservationId);
                }
            }
        }

        private void SchedulePaymentEmail(int reservationId, string email, string paymentLink)
        {
            // Queue background task to send email
            backgroundTaskQueue.QueueBackgroundWorkItem(async (cancellationToken) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var emailContentService = scope.ServiceProvider.GetRequiredService<IEmailContentService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                logger.Information("Starting approval email task for reservation {ReservationId}.", reservationId);
                try
                {
                    // Send email
                    var emailBody = await emailContentService.GeneratePaymentEmailAsync(
                        reservationId,
                        email,
                        paymentLink,
                        cancellationToken);
                    await emailService.SendEmailAsync(
                        email,
                        "Reservation Approved - Complete Payment",
                        emailBody);
                    logger.Information("Approval email with payment link sent for reservation {ReservationId}", reservationId);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error sending payment confirmation email to {Email}.", email);
                }

                logger.Information("Scheduled approval email for reservation {ReservationId}.", reservationId);
            });
        }

        private void SchedulePaymentConfirmationEmail(int reservationId, int reservationUserId)
        {
            // Queue background task to send email
            backgroundTaskQueue.QueueBackgroundWorkItem(async (cancellationToken) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                var userRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<ReservationUserDetail, int>>();
                var emailContentService = scope.ServiceProvider.GetRequiredService<IEmailContentService>();

                logger.Information("Sending payment confirmation email for reservation {ReservationId}.", reservationId);

                try
                {
                    // Get user email
                    var user = await userRepository.GetByIdAsync(reservationUserId, cancellationToken);
                    if (user == null || string.IsNullOrEmpty(user.Email))
                    {
                        logger.Warning("User not found or email missing for ReservationUserID: {UserId}", reservationUserId);
                        return;
                    }

                    // Generate and send email
                    var emailBody = await emailContentService.GeneratePaymentConfirmationEmailBodyAsync(reservationId, cancellationToken);
                    await emailService.SendEmailAsync(user.Email, "Payment Confirmation", emailBody);
                    logger.Information("Payment confirmation email sent for reservation {ReservationId}.", reservationId);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error sending payment confirmation email for reservation {ReservationId}.", reservationId);
                }
            });

            logger.Information("Scheduled payment confirmation email for reservation {ReservationId}.", reservationId);
        }

        private void ScheduleCancellationEmail(int reservationId, string email)
        {
            // Queue background task to send email
            backgroundTaskQueue.QueueBackgroundWorkItem(async (cancellationToken) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var emailContentService = scope.ServiceProvider.GetRequiredService<IEmailContentService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                logger.Information("Starting cancellation email task for reservation {ReservationId}.", reservationId);
                try
                {
                    // Send email
                    var emailBody = await emailContentService.GenerateReservationCancellationEmailAsync(
                        reservationId,
                        email,
                        cancellationToken);
                    await emailService.SendEmailAsync(
                        email,
                        "Reservation Cancelled",
                        emailBody);
                    logger.Information("Cancellation email sent for reservation {ReservationId}", reservationId);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error sending cancellation email to {Email}.", email);
                }

                logger.Information("Scheduled cancellation email for reservation {ReservationId}.", reservationId);
            });
        }
    }
}
