using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.ApproveDocument
{
    public class ApproveDocumentCommandHandler(
            IGenericRepository<Reservation, int> reservationRepository,
            IGenericRepository<Payment, Guid> paymentRepository,
            IGenericRepository<Document, int> documentRepository,
            IGenericRepository<Room, int> roomRepository,
            IUnitOfWork unitOfWork,
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
    }
}
