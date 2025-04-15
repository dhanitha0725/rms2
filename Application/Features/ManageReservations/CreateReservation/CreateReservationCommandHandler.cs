using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.CreateReservation
{
    public class CreateReservationCommandHandler(
                IGenericRepository<User, int> userRepository,
                IGenericRepository<Reservation, int> reservationRepository,
                IGenericRepository<ReservationUserDetail, int> reservationUserRepository,
                IGenericRepository<ReservedPackage, int> reservedPackageRepository,
                IGenericRepository<ReservedRoom, int> reservedRoomRepository,
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
                    StartDate = request.StartDate.ToLocalTime(),
                    EndDate = request.EndDate.ToLocalTime(),
                    UserType = request.CustomerType,
                    Total = request.Total,
                    Status = string.Equals(request.CustomerType, "private", StringComparison.OrdinalIgnoreCase)
                        ? ReservationStatus.PendingPayment
                        : ReservationStatus.PendingApproval,
                    CreatedDate = DateTime.UtcNow,
                };

                if (!await TryAddAsync(reservationRepository, reservation, cancellationToken))
                {
                    return Result<ReservationResultDto>.Failure(new Error("Failed to create reservation."));
                }

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

                if (!await TryAddAsync(reservationUserRepository, reservationUserDetails, cancellationToken))
                {
                    return Result<ReservationResultDto>.Failure(new Error("Failed to create reservation user details."));
                }

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

                        if (!await TryAddAsync(reservedPackageRepository, reservedPackage, cancellationToken))
                        {
                            return Result<ReservationResultDto>.Failure(new Error($"Failed to reserve package with ID {item.ItemId}."));
                        }

                        logger.Information("Reserved package with ID {PackageID} for reservation {ReservationID}.", item.ItemId, reservation.ReservationID);
                    }
                    else
                    {
                        var reservedRoom = new ReservedRoom
                        {
                            ReservationID = reservation.ReservationID,
                            RoomID = item.ItemId,
                            StartDate = request.StartDate.ToLocalTime(),
                            EndDate = request.EndDate.ToLocalTime(),
                        };

                        if (!await TryAddAsync(reservedRoomRepository, reservedRoom, cancellationToken))
                        {
                            return Result<ReservationResultDto>.Failure(new Error($"Failed to reserve room with ID {item.ItemId}."));
                        }

                        logger.Information("Reserved room with ID {RoomID} for reservation {ReservationID}.", item.ItemId, reservation.ReservationID);
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ReservationResultDto>.Success(new ReservationResultDto
                {
                    ReservationId = reservation.ReservationID,
                    TotalPrice = reservation.Total,
                    Status = reservation.Status.ToString(),
                });
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error creating reservation");
                return Result<ReservationResultDto>.Failure(new Error("An error occurred while creating the reservation."));
            }
        }

        private async Task<bool> TryAddAsync<T>(IGenericRepository<T, int> repository, T entity, CancellationToken cancellationToken) where T : class
        {
            try
            {
                await repository.AddAsync(entity, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error adding entity of type {EntityType}.", typeof(T).Name);
                return false;
            }
        }
    }
}
