using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CheckAvailability
{
    public class CheckAvailabilityQueryHandler (
        IGenericRepository<ReservedPackage, int> reservedPackageRepository,
        IGenericRepository<Room,int> roomRepository,
        IGenericRepository<Package, int> packageRepository,
        ILogger logger) 
        : IRequestHandler<CheckAvailabilityQuery, Result<bool>>
    {
        public async Task<Result<bool>> Handle(
            CheckAvailabilityQuery request, 
            CancellationToken cancellationToken)
        {
            foreach (var selectedPackage in request.Packages)
            {
                var package = await packageRepository.GetByIdAsync(selectedPackage.PackageId, cancellationToken);
                if (package == null || package.FacilityID != request.FacilityID)
                {
                    logger.Error($"PackageId {selectedPackage.PackageId} not found");
                    return Result<bool>.Failure(new Error("Package not found"));
                }
                
                //check existing reservations for the package
                var reservedPackages = await reservedPackageRepository.GetAllAsync(cancellationToken);
                var reservedQty = reservedPackages
                    .Where(rp => rp.PackageID == selectedPackage.PackageId &&
                                 rp.Reservation.StartDate < request.StartDate &&
                                 rp.Reservation.EndDate > request.StartDate &&
                                 rp.Reservation.Status != "Canceled")
                    .Sum(rp => rp.Quantity);

                // check available rooms if package id includes room
                var totalRooms = (await roomRepository.GetAllAsync(cancellationToken))
                    .Count(r => r.FacilityID == request.FacilityID && r.Status == "Available");

                // each package quantity requires a room
                var requiredCapacity = selectedPackage.Quantity;
                if (totalRooms - reservedQty < requiredCapacity)
                {
                    logger.Error($"Not enough rooms available for package {selectedPackage.PackageId}");
                    return Result<bool>.Failure(new Error("Not enough rooms available"));
                }
            }

            return Result<bool>.Success(true);
        }
    }
}
