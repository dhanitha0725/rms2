using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.UpdateReservation
{
    public class UpdateReservationCommandHandler(
        IGenericRepository<Reservation, int> reservationRepository,
        IGenericRepository<ReservedPackage, int> reservedPackageRepository,
        IGenericRepository<ReservedRoom, int> reservedRoomRepository,
        IGenericRepository<Room, int> roomRepository,
        IUnitOfWork unitOfWork,
        ILogger logger)
        : IRequestHandler<UpdateReservationCommand, Result<ReservationResultDto>>
    {
        public async Task<Result<ReservationResultDto>> Handle(
            UpdateReservationCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Get the reservation
                var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);

                // Check if reservation state
                if (reservation.Status is
                ReservationStatus.Cancelled or
                ReservationStatus.Completed or
                ReservationStatus.Confirmed or
                ReservationStatus.Expired)
                {
                    logger.Warning("Cannot update reservation {ReservationId} with status {Status}",
                        request.ReservationId, reservation.Status);
                    return Result<ReservationResultDto>.Failure(
                        new Error($"Cannot update reservation in {reservation.Status} status"));
                }

                // Store original values for comparison
                var originalStartDate = reservation.StartDate;
                var originalEndDate = reservation.EndDate;

                // Update reservation dates if provided
                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    reservation.StartDate = request.StartDate.Value;
                    reservation.EndDate = request.EndDate.Value;
                    reservation.UpdatedDate = DateTime.UtcNow;
                    await reservationRepository.UpdateAsync(reservation, cancellationToken);
                    logger.Information("Updated reservation {ReservationId} dates: {StartDate} to {EndDate}",
                        reservation.ReservationID, reservation.StartDate, reservation.EndDate);
                }

                // Handle packages update
                if (request.PackageUpdates != null && request.PackageUpdates.Any())
                {
                    // Get existing reserved packages
                    var existingPackages = await reservedPackageRepository.GetAllAsync(
                        rp => rp.ReservationID == reservation.ReservationID,
                        cancellationToken);

                    // Remove packages not in the update list
                    var packagesToRemove = existingPackages.Where(ep =>
                        !request.PackageUpdates.Any(up => up.ItemId == ep.PackageID)).ToList();

                    foreach (var packageToRemove in packagesToRemove)
                    {
                        await reservedPackageRepository.DeleteAsync(packageToRemove.ReservedPackageID, cancellationToken);
                        logger.Information("Removed package {PackageId} from reservation {ReservationId}",
                            packageToRemove.PackageID, reservation.ReservationID);
                    }

                    // Add new packages or update existing ones
                    foreach (var packageUpdate in request.PackageUpdates)
                    {
                        var existingPackage = existingPackages.FirstOrDefault(ep => ep.PackageID == packageUpdate.ItemId);

                        if (existingPackage == null)
                        {
                            // Add new package
                            var newReservedPackage = new ReservedPackage
                            {
                                ReservationID = reservation.ReservationID,
                                PackageID = packageUpdate.ItemId,
                                status = "reserved",
                                StartDate = reservation.StartDate,
                                EndDate = reservation.EndDate
                            };

                            await reservedPackageRepository.AddAsync(newReservedPackage, cancellationToken);
                            logger.Information("Added new package {PackageId} to reservation {ReservationId}",
                                packageUpdate.ItemId, reservation.ReservationID);
                        }
                        else
                        {
                            // Update existing package dates if they changed
                            if (reservation.StartDate != originalStartDate || reservation.EndDate != originalEndDate)
                            {
                                existingPackage.StartDate = reservation.StartDate;
                                existingPackage.EndDate = reservation.EndDate;
                                await reservedPackageRepository.UpdateAsync(existingPackage, cancellationToken);
                                logger.Information("Updated package {PackageId} dates for reservation {ReservationId}",
                                    existingPackage.PackageID, reservation.ReservationID);
                            }
                        }
                    }
                }

                // Handle rooms update
                if (request.RoomUpdates != null && request.RoomUpdates.Any())
                {
                    // Get all current reserved rooms
                    var existingReservedRooms = await reservedRoomRepository.GetAllAsync(
                        rr => rr.ReservationID == reservation.ReservationID,
                        cancellationToken);

                    // Release all currently reserved rooms
                    foreach (var reservedRoom in existingReservedRooms)
                    {
                        var room = await roomRepository.GetByIdAsync(reservedRoom.RoomID, cancellationToken);
                        if (room != null)
                        {
                            room.Status = "Available";
                            await roomRepository.UpdateAsync(room, cancellationToken);
                            logger.Information("Released room {RoomId} for update in reservation {ReservationId}",
                                room.RoomID, reservation.ReservationID);
                        }

                        await reservedRoomRepository.DeleteAsync(reservedRoom.ReservedRoomID, cancellationToken);
                    }

                    // Add new room reservations
                    foreach (var roomUpdate in request.RoomUpdates)
                    {
                        // Find available rooms of the requested type
                        var availableRooms = await roomRepository.GetAllAsync(cancellationToken);
                        var matchingRooms = availableRooms
                            .Where(r => r.RoomTypeID == roomUpdate.ItemId &&
                                     r.Status == "Available")
                            .Take(roomUpdate.Quantity)
                            .ToList();

                        if (matchingRooms.Count < roomUpdate.Quantity)
                        {
                            logger.Warning("Insufficient available rooms of type {RoomTypeId} for reservation {ReservationId}",
                                roomUpdate.ItemId, reservation.ReservationID);
                            await unitOfWork.RollbackTransactionAsync(cancellationToken);
                            return Result<ReservationResultDto>.Failure(new Error("Insufficient available rooms"));
                        }

                        foreach (var room in matchingRooms)
                        {
                            var reservedRoom = new ReservedRoom
                            {
                                ReservationID = reservation.ReservationID,
                                RoomID = room.RoomID,
                                StartDate = reservation.StartDate,
                                EndDate = reservation.EndDate,
                            };

                            await reservedRoomRepository.AddAsync(reservedRoom, cancellationToken);
                            logger.Information("Reserved room {RoomId} for reservation {ReservationId}",
                                room.RoomID, reservation.ReservationID);

                            room.Status = "Reserved";
                            await roomRepository.UpdateAsync(room, cancellationToken);
                            logger.Information("Updated room {RoomId} status to 'Reserved'", room.RoomID);
                        }
                    }
                }

                // Update total price if provided
                if (request.Total.HasValue && request.Total.Value > 0)
                {
                    reservation.Total = request.Total.Value;
                    await reservationRepository.UpdateAsync(reservation, cancellationToken);
                    logger.Information("Updated total price for reservation {ReservationId} to {Total}",
                        reservation.ReservationID, reservation.Total);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReservationResultDto>.Success(new ReservationResultDto
                {
                    ReservationId = reservation.ReservationID,
                    TotalPrice = reservation.Total,
                    Status = reservation.Status.ToString(),
                    OrderId = reservation.ReservationID.ToString()
                });
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(ex, "Error updating reservation {ReservationId}", request.ReservationId);
                return Result<ReservationResultDto>.Failure(new Error("An error occurred while updating the reservation"));
            }
        }
    }
}
