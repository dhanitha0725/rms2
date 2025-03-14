using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CreateReservation
{
    public class CreateReservationCommandHandler (
        IGenericRepository<Reservation, int> reservationRepository,
        IGenericRepository<ReservedPackage, int> reservedPackgeRepository,
        IGenericRepository<ReservedRoom, int> reservedRoomRepository,
        IGenericRepository<Room, int> roomRepository,
        IUnitOfWork unitOfWork,
        ILogger logger) 
        : IRequestHandler<CreateReservationCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            CreateReservationCommand request, 
            CancellationToken cancellationToken)
        {
            try
            {
                // Create reservation
                var reservation = new Reservation
                {
                    UserID = request.UserId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    CreatedDate = DateTime.UtcNow,
                    Total = request.Total,
                    Status = request.CustomerType == "Private" ? "PendingPayment" : "PendingApproval",
                    UserType = request.CustomerType,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = null
                };

                // Add reservation to database
                await reservationRepository.AddAsync(reservation, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                // Add reserved packages and rooms
                foreach (var package in request.Packages)
                {
                    var reservedPackage = new ReservedPackage
                    {
                        PackageID = package.PackageId,
                        ReservationID = reservation.ReservationID,
                        Quantity = package.Quantity
                    };

                    await reservedPackgeRepository.AddAsync(reservedPackage, cancellationToken);


                    //Problem: The current room allocation logic queries
                    //available rooms and marks them as "Reserved" without atomic checks,
                    //risking overbooking in concurrent scenarios.
                    var availableRooms = (await roomRepository.GetAllAsync(cancellationToken))
                        .Where(r => r.FacilityID == request.FacilityId && r.Status == "Available")
                        .Take(package.Quantity);

                    // assign rooms if applicable
                    foreach (var room in availableRooms)
                    {
                        var reservedRoom = new ReservedRoom
                        {
                            RoomID = room.RoomID,
                            ReservationID = reservation.ReservationID
                        };
                        await reservedRoomRepository.AddAsync(reservedRoom, cancellationToken);
                        room.Status = "Reserved";
                        await roomRepository.UpdateAsync(room, cancellationToken);
                    }
                }
                await unitOfWork.SaveChangesAsync(cancellationToken);

                logger.Information("Reservation created successfully");
                return Result<int>.Success(reservation.ReservationID);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error creating reservation");
                throw;
            }
        }
    }
}
