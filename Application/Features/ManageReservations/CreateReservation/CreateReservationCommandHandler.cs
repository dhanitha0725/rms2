using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Application.EventHandlers;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application.Features.ManageReservations.CreateReservation
{
    public class CreateReservationCommandHandler(
                IGenericRepository<Reservation, int> reservationRepository,
                IGenericRepository<ReservationUserDetail, int> reservationUserRepository,
                IGenericRepository<ReservedPackage, int> reservedPackageRepository,
                IGenericRepository<ReservedRoom, int> reservedRoomRepository,
                IGenericRepository<Room, int> roomRepository,
                IBackgroundTaskQueue backgroundTaskQueue,
                IServiceScopeFactory serviceScopeFactory,
                IUnitOfWork unitOfWork,
                ILogger logger)
                : IRequestHandler<CreateReservationCommand, Result<ReservationResultDto>>
    {
        public async Task<Result<ReservationResultDto>> Handle(
            CreateReservationCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Create reservation
                var reservation = new Reservation
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    UserType = request.CustomerType,
                    Total = request.Total,
                    Status = string.Equals(request.CustomerType, "private", StringComparison.OrdinalIgnoreCase)
                        ? ReservationStatus.PendingPayment
                        : ReservationStatus.PendingApproval,
                    CreatedDate = DateTime.UtcNow,
                };

                await reservationRepository.AddAsync(reservation, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                // Create user details
                var reservationUserDetails = new ReservationUserDetail
                {
                    ReservationID = reservation.ReservationID,
                    FirstName = request.UserDetails.FirstName,
                    LastName = request.UserDetails.LastName,
                    Email = request.UserDetails.Email,
                    PhoneNumber = request.UserDetails.PhoneNumber,
                    OrganizationName = request.UserDetails.OrganizationName,
                };

                await reservationUserRepository.AddAsync(reservationUserDetails, cancellationToken);

                // Create reserved items
                foreach (var item in request.Items)
                {
                    if (item.Type == "package")
                    {
                        var reservedPackage = new ReservedPackage
                        {
                            ReservationID = reservation.ReservationID,
                            PackageID = item.ItemId,
                            status = "reserved"
                        };

                        await reservedPackageRepository.AddAsync(reservedPackage, cancellationToken);

                        logger.Information("Reserved package with ID {PackageID} for reservation {ReservationID}.",
                            item.ItemId, reservation.ReservationID);
                    }
                    else
                    {
                        // Fetch available rooms for the given RoomTypeID and FacilityID
                        var availableRooms = await roomRepository.GetAllAsync(cancellationToken);
                        // Filter rooms based on the request
                        var matchingRooms = availableRooms
                            .Where(r => r.RoomTypeID == item.ItemId &&
                                        r.FacilityID == request.FacilityId &&
                                        r.Status == "Available")
                            .Take(item.Quantity)
                            .ToList();

                        if (matchingRooms.Count < item.Quantity)
                        {
                            logger.Warning("Insufficient available rooms of type {RoomTypeID} in facility {FacilityID}.",
                                item.ItemId, request.FacilityId);
                            return Result<ReservationResultDto>.Failure(new Error("Insufficient available rooms."));
                        }

                        // Allocate the rooms
                        foreach (var room in matchingRooms)
                        {
                            var reservedRoom = new ReservedRoom
                            {
                                ReservationID = reservation.ReservationID,
                                RoomID = room.RoomID,
                                StartDate = request.StartDate,
                                EndDate = request.EndDate,
                            };

                            await reservedRoomRepository.AddAsync(reservedRoom, cancellationToken);

                            // Update room status to "Reserved"
                            room.Status = "Reserved";
                            await roomRepository.UpdateAsync(room, cancellationToken);

                            logger.Information("Reserved room with ID {RoomID} for reservation {ReservationID}.",
                                room.RoomID, reservation.ReservationID);
                        }
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                if (reservation.Status == ReservationStatus.PendingPayment)
                {
                    // scheduled task: change status to expired after 30 minutes (for pending payments)
                    ScheduleReservationExpiration(reservation.ReservationID);
                }

                return Result<ReservationResultDto>.Success(new ReservationResultDto
                {
                    ReservationId = reservation.ReservationID,
                    TotalPrice = reservation.Total,
                    Status = reservation.Status.ToString(),
                    OrderId = reservation.ReservationID.ToString()
                });
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error creating reservation");
                return Result<ReservationResultDto>.Failure(new Error("An error occurred while creating the reservation."));
            }
        }

        private void ScheduleReservationExpiration(int reservationId)
        {
            // Queue the expiration task to run after 30 minutes
            backgroundTaskQueue.QueueBackgroundWorkItem(async (cancellationToken) =>
            {
                // Wait for 30 minutes
                await Task.Delay(TimeSpan.FromMinutes(30), cancellationToken);

                // Create the expiration task
                var expireTask = new ExpireReservationTask(
                    reservationId,
                    serviceScopeFactory,
                    logger);

                // Execute the task
                await expireTask.Execute(cancellationToken);
            });

            logger.Information("Scheduled expiration for reservation {ReservationId} in 30 minutes.", reservationId);
        }
    }
}
