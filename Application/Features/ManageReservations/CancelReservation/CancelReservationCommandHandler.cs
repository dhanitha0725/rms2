using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application.Features.ManageReservations.CancelReservation;

public class CancelReservationCommandHandler(
    IGenericRepository<Reservation, int> reservationRepository,
    IGenericRepository<Payment, Guid> paymentRepository,
    IGenericRepository<ReservedRoom, int> reservedRoomRepository,
    IGenericRepository<Room, int> roomRepository,
    IGenericRepository<ReservationUserDetail, int> reservationUserRepository,
    IUnitOfWork unitOfWork,
    IBackgroundTaskQueue backgroundTaskQueue,
    IServiceScopeFactory serviceScopeFactory,
    ILogger logger)
    : IRequestHandler<CancelReservationCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CancelReservationCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Get the reservation 
            var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);

            // update reservation status to Cancelled
            reservation.Status = ReservationStatus.Cancelled;
            reservation.UpdatedDate = DateTime.UtcNow;
            await reservationRepository.UpdateAsync(reservation, cancellationToken);
            logger.Information("Reservation {ReservationId} status updated to Cancelled.", reservation.ReservationID);

            // Release all reserved rooms
            await ReleaseRoomsForReservationAsync(reservation.ReservationID, cancellationToken);
            logger.Information("All rooms released for reservation {ReservationId}.", reservation.ReservationID);

            // Update associated payment status
            var payments = await paymentRepository.GetAllAsync(
                p => p.ReservationID == reservation.ReservationID,
                cancellationToken);

            foreach (var payment in payments)
            {
                payment.Status = "Cancelled";
                await paymentRepository.UpdateAsync(payment, cancellationToken);
                logger.Information("Payment {PaymentId} status updated to Cancelled.", payment.PaymentID);
            }

            // Find user to send cancellation email
            var reservationUser = await reservationUserRepository.GetAllAsync(
                ru => ru.ReservationID == reservation.ReservationID,
                cancellationToken);

            var user = reservationUser.FirstOrDefault();
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                // Schedule cancellation email
                ScheduleCancellationEmail(reservation.ReservationID, user.Email);
                logger.Information("Cancellation email scheduled for reservation {ReservationId}.", reservation.ReservationID);
            }
            else
            {
                logger.Warning("No user email found for reservation {ReservationId}. Cancellation email not sent.", reservation.ReservationID);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<int>.Success(reservation.ReservationID);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            logger.Error(ex, "Error cancelling reservation {ReservationId}", request.ReservationId);
            return Result<int>.Failure(new Error("An error occurred while cancelling the reservation."));
        }
    }

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

        // Update each room's status to Available
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
        });

        logger.Information("Scheduled cancellation email for reservation {ReservationId}.", reservationId);
    }
}
