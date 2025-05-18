using Application.Abstractions.Interfaces;
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
            IUnitOfWork unitOfWork,
            IBackgroundTaskQueue backgroundTaskQueue,
            IServiceScopeFactory serviceScopeFactory,
            IEmailContentService emailContentService,
            IConfiguration configuration,
            ILogger logger)
            : IRequestHandler<ApproveDocumentCommand, Result>
    {
        public async Task<Result> Handle(
            ApproveDocumentCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Retrieve the document
                var document = await documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
                if (document == null)
                    return Result.Failure(new Error("Document Not Found"));

                if (request.IsApproved)
                {
                    // Handle ApprovalDocument
                    if (request.DocumentType == nameof(DocumentType.ApprovalDocument))
                    {
                        if (document.ReservationId == null)
                            return Result.Failure(new Error("Document is not associated with a reservation."));

                        var reservation = await reservationRepository.GetByIdAsync(document.ReservationId.Value, cancellationToken);
                        if (reservation == null)
                            return Result.Failure(new Error("Reservation not found"));

                        if (reservation.Status != ReservationStatus.PendingApproval)
                            return Result.Failure(new Error("Reservation is not in PendingApproval status."));

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
                            return Result.Failure(new Error("Reservation user details not found"));
                        }

                        // create payment record
                        var payment = new Payment
                        {
                            OrderID = request.OrderId,
                            AmountPaid = reservation.Total,
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
                        var baseUrl = configuration["PayhereSettings:BaseUrl"]; ;
                        var paymentLink = $"{baseUrl}/api/payments/initiate?orderId=";

                        // schedule email
                        SchedulePaymentEmail(reservation.ReservationID, reservationUserDetail.Email, paymentLink);
                    }
                    // Handle BankReceipt
                    else if (request.DocumentType == nameof(DocumentType.BankReceipt))
                    {
                        if (document.PaymentId == null)
                            return Result.Failure(new Error("Document is not associated with a payment."));

                        var payment = await paymentRepository.GetByIdAsync(document.PaymentId.Value, cancellationToken);
                        if (payment == null)
                            return Result.Failure(new Error("Payment not found."));

                        var reservation = await reservationRepository.GetByIdAsync(payment.ReservationID, cancellationToken);
                        if (reservation == null)
                            return Result.Failure(new Error("Reservation not found."));

                        if (reservation.Status != ReservationStatus.PendingPaymentVerification)
                            return Result.Failure(new Error("Reservation is not in PendingPaymentVerification status."));

                        payment.AmountPaid = request.AmountPaid;
                        payment.Status = "Completed";
                        await paymentRepository.UpdateAsync(payment, cancellationToken);
                        logger.Information("Payment {PaymentId} status updated to Completed after document approval.", payment.PaymentID);

                        reservation.Status = ReservationStatus.Confirmed;
                        reservation.UpdatedDate = DateTime.UtcNow;
                        await reservationRepository.UpdateAsync(reservation, cancellationToken);
                        logger.Information("Reservation {ReservationId} status updated to Confirmed after payment completion.", reservation.ReservationID);
                    }
                    else
                    {
                        return Result.Failure(new Error("Invalid Document Type"));
                    }
                }
                else
                {
                    // Handle document rejection
                    if (request.DocumentType == nameof(DocumentType.BankReceipt))
                    {
                        if (document.PaymentId == null)
                            return Result.Failure(new Error("Document is not associated with a payment."));

                        var payment = await paymentRepository.GetByIdAsync(document.PaymentId.Value, cancellationToken);
                        if (payment == null)
                            return Result.Failure(new Error("Payment not found."));

                        var reservation = await reservationRepository.GetByIdAsync(payment.ReservationID, cancellationToken);
                        if (reservation == null)
                            return Result.Failure(new Error("Reservation not found."));

                        payment.Status = "Cancelled";
                        await paymentRepository.UpdateAsync(payment, cancellationToken);

                        reservation.Status = ReservationStatus.Cancelled;
                        reservation.UpdatedDate = DateTime.UtcNow;
                        await reservationRepository.UpdateAsync(reservation, cancellationToken);
                        logger.Information("Reservation {ReservationId} and Payment {PaymentId} status updated to Cancelled after bank receipt rejection.", reservation.ReservationID, payment.PaymentID);

                        // Release associated rooms
                        // (available status is not updating)
                        var reservedRooms = reservation.ReservedRooms;
                        if (reservedRooms != null)
                        {
                            foreach (var reservedRoom in reservedRooms)
                            {
                                var room = await roomRepository.GetByIdAsync(reservedRoom.RoomID, cancellationToken);
                                if (room != null)
                                {
                                    room.Status = "Available";
                                    await roomRepository.UpdateAsync(room, cancellationToken);
                                    logger.Information("Room {RoomId} status updated to Available after reservation cancellation.", room.RoomID);
                                }
                            }
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

                                // Release associated rooms
                                var reservedRooms = reservation.ReservedRooms;
                                if (reservedRooms != null)
                                {
                                    foreach (var reservedRoom in reservedRooms)
                                    {
                                        var room = await roomRepository.GetByIdAsync(reservedRoom.RoomID, cancellationToken);
                                        if (room != null)
                                        {
                                            room.Status = "Available";
                                            await roomRepository.UpdateAsync(room, cancellationToken);
                                            logger.Information("Room {RoomId} status updated to Available after reservation cancellation.", room.RoomID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success([]);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error processing document approval");
                return Result.Failure(new Error("An error occurred while processing the request."));
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
                logger.Information("Sending payment confirmation email for reservation {ReservationId}.", reservationId);
                try
                {
                    // Send email
                    var emailBody = await emailContentService.GeneratePaymentEmailAsync(reservationId, email, paymentLink, cancellationToken);
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
    }
}
