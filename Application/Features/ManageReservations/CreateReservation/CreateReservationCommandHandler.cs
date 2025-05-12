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
                IGenericRepository<Payment,int> paymentRepository,
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
                // Determine reservation status based on customer type and payment method
                ReservationStatus status;
                if (string.Equals(request.CustomerType, "private", StringComparison.OrdinalIgnoreCase))
                {
                    if (request is { PaymentMethod: nameof(PaymentMethods.Cash), IsPaymentReceived: true })
                    {
                        status = ReservationStatus.Confirmed;
                    }
                    else
                    {
                        status = request.PaymentMethod switch
                        {
                            nameof(PaymentMethods.Bank) => ReservationStatus.PendingPaymentVerification,
                            nameof(PaymentMethods.Cash) => ReservationStatus.PendingCashPayment,
                            _ => throw new ArgumentException("Invalid payment method")
                        };
                    }
                }
                else
                {
                    status = ReservationStatus.PendingApproval;
                }

                // Create reservation
                var reservation = new Reservation
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    UserType = request.CustomerType,
                    Total = request.Total,
                    Status = status,
                    CreatedDate = DateTime.UtcNow,
                };

                await reservationRepository.AddAsync(reservation, cancellationToken);
                logger.Information("Reservation created with ID {ReservationID}.", reservation.ReservationID);
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
                logger.Information("Reservation user details created for ReservationID {ReservationID}.", reservation.ReservationID);
                await unitOfWork.SaveChangesAsync(cancellationToken);

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
                        var availableRooms = await roomRepository.GetAllAsync(cancellationToken);
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
                            logger.Information("Reserved room with ID {RoomID} for reservation {ReservationID}.",
                                room.RoomID, reservation.ReservationID);

                            room.Status = "Reserved";
                            await roomRepository.UpdateAsync(room, cancellationToken);
                            logger.Information("Room with ID {RoomID} status updated to 'Reserved'.", room.RoomID);
                        }
                    }
                }

                // Create payment record for non-online payments
                Payment? payment = null;
                if (request.PaymentMethod is nameof(PaymentMethods.Bank) or nameof(PaymentMethods.Cash))
                {
                    payment = new Payment
                    {
                        Method = request.PaymentMethod,
                        AmountPaid = request is { PaymentMethod: nameof(PaymentMethods.Cash), IsPaymentReceived: true } ? request.Total : null,
                        Currency = "LKR",
                        CreatedDate = DateTime.UtcNow,
                        Status = request is { PaymentMethod: nameof(PaymentMethods.Cash), IsPaymentReceived: true } ? "Completed" : "Pending",
                        ReservationID = reservation.ReservationID,
                        ReservationUserID = reservationUserDetails.ReservationUserDetailID,
                    };

                    await paymentRepository.AddAsync(payment, cancellationToken);
                    logger.Information("Payment record created for ReservationID {ReservationID} with status {PaymentStatus}.",
                        reservation.ReservationID, payment.Status);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.Information("All changes saved to the database for ReservationID {ReservationID}.", reservation.ReservationID);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                // expire for late online payments
                if (reservation.Status == ReservationStatus.PendingPayment)
                {
                    ScheduleReservationExpiration(reservation.ReservationID);
                }

                // cancel for late cash payments
                if (reservation.Status == ReservationStatus.PendingCashPayment)
                {
                    ScheduleReservationExpirationForCashPayments(reservation.ReservationID);
                }

                return Result<ReservationResultDto>.Success(new ReservationResultDto
                {
                    ReservationId = reservation.ReservationID,
                    TotalPrice = reservation.Total,
                    Status = reservation.Status.ToString(),
                    OrderId = reservation.ReservationID.ToString(),
                    PaymentId = payment?.PaymentID
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

        private void ScheduleReservationExpirationForCashPayments(int reservationId)
        {
            // Queue the expiration task to run after 2 days
            backgroundTaskQueue.QueueBackgroundWorkItem(async (cancellationToken) =>
            {
                // Wait for 2 days
                await Task.Delay(TimeSpan.FromDays(1), cancellationToken);

                // Use a new scope to resolve services
                using var scope = serviceScopeFactory.CreateScope();
                var reservationRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Reservation, int>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();

                try
                {
                    // Retrieve the reservation
                    var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
                    if (reservation == null)
                    {
                        logger.Warning("Reservation with ID {ReservationId} not found for cancellation.", reservationId);
                        return;
                    }

                    // Update the reservation status
                    reservation.Status = ReservationStatus.Cancelled;

                    // Save the changes
                    await reservationRepository.UpdateAsync(reservation, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    logger.Information("Reservation with ID {ReservationId} has been cancelled.", reservationId);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error cancelling reservation with ID {ReservationId}.", reservationId);
                }
            });

            logger.Information("Scheduled cancellation for reservation {ReservationId} in 2 days.", reservationId);
        }
    }
}
