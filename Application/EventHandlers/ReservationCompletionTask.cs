using Application.Abstractions.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application.EventHandlers
{
    public class ReservationCompletionTask(
                        IServiceScopeFactory serviceScopeFactory,
                        ILogger logger)
    {
        public async Task Execute(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            try
            {
                // Create a new scope to resolve services
                using var scope = serviceScopeFactory.CreateScope();
                var reservationRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Reservation, int>>();
                var roomRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Room, int>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                await unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Handle confirmed reservations past end date
                    var reservationsToComplete = await reservationRepository.GetAllAsync(
                        r => r.EndDate < now && r.Status == ReservationStatus.Confirmed,
                        cancellationToken);

                    foreach (var reservation in reservationsToComplete)
                    {
                        try
                        {
                            // Update reservation status to "Completed"
                            reservation.Status = ReservationStatus.Completed;
                            reservation.UpdatedDate = DateTime.UtcNow;
                            await reservationRepository.UpdateAsync(reservation, cancellationToken);
                            logger.Information("Reservation {ReservationId} status updated to Completed.",
                                reservation.ReservationID);

                            // Release associated rooms
                            var reservedRooms = await roomRepository.GetAllAsync(
                                room => room.ReservedRooms.Any(rr => rr.ReservationID == reservation.ReservationID),
                                cancellationToken);

                            foreach (var room in reservedRooms)
                            {
                                room.Status = "Available";
                                await roomRepository.UpdateAsync(room, cancellationToken);
                                logger.Information("Room {RoomId} status updated to Available after reservation completion.",
                                    room.RoomID);
                            }

                            logger.Information("Reservation {ReservationId} completed and rooms released.",
                                reservation.ReservationID);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Error completing reservation {ReservationId}.",
                                reservation.ReservationID);
                            throw;
                        }
                    }

                    // Release rooms for already expired or cancelled reservations
                    var expiredOrCancelledReservations = await reservationRepository.GetAllAsync(
                        r => r.Status == ReservationStatus.Expired || r.Status == ReservationStatus.Cancelled,
                        cancellationToken);

                    foreach (var reservation in expiredOrCancelledReservations)
                    {
                        try
                        {
                            // find rooms that are still marked as reserved for reservation
                            var reservedRooms = await roomRepository.GetAllAsync(
                                room => room.ReservedRooms.Any(rr => rr.ReservationID == reservation.ReservationID) &&
                                        room.Status != "Available",
                                cancellationToken);

                            if (reservedRooms.Any())
                            {
                                foreach (var room in reservedRooms)
                                {
                                    room.Status = "Available";
                                    await roomRepository.UpdateAsync(room, cancellationToken);
                                    logger.Information("Room {RoomId} status updated to Available for {Status} reservation {ReservationId}.",
                                        room.RoomID, reservation.Status, reservation.ReservationID);
                                }

                                logger.Information("Released {Count} rooms for {Status} reservation {ReservationId}.",
                                    reservedRooms.Count(), reservation.Status, reservation.ReservationID);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Error releasing rooms for {Status} reservation {ReservationId}.",
                                reservation.Status, reservation.ReservationID);
                            throw;
                        }
                    }

                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    await unitOfWork.CommitTransactionAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    logger.Error(ex, "Transaction rolled back due to an error.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error processing reservation completion task.");
                throw;
            }
        }
    }
}
