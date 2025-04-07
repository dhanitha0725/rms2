using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.CheckAvailability
{
    public class CheckAvailabilityQueryHandler(
        IGenericRepository<Facility, int> facilityRepository,
        IGenericRepository<Package, int> packageRepository,
        IRoomRepository roomRepository,
        IReservationRepository reservationRepository,
        ILogger logger)
        : IRequestHandler<CheckAvailabilityQuery, Result<AvailabilityResultDto>>
    {
        public async Task<Result<AvailabilityResultDto>> Handle(
            CheckAvailabilityQuery request,
            CancellationToken cancellationToken)
        {
            var result = new AvailabilityResultDto();

            // check facility availability
            var facility = await facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
            //if (facility == null)
            //{
            //    result.IsAvailable = false;
            //    return Result<AvailabilityResultDto>.Failure(new Error("Facility not found"));
            //}

            // check room availability
            if (request.Rooms.Any())
            {
                if (!facility.HasRooms)
                {
                    result.IsAvailable = false;
                    return Result<AvailabilityResultDto>.Failure(new Error("Facility does not have rooms"));
                }
            }

            foreach (var roomRequest in request.Rooms)
            {
                var totalRooms = await roomRepository.GetRoomCountByTypeAsync(
                    request.FacilityId,
                    roomRequest.RoomType);

                if (totalRooms == 0)
                {
                    result.IsAvailable = false;
                    return Result<AvailabilityResultDto>.Failure(
                        new Error($"Room type {roomRequest.RoomType} not found"));
                }

                var reservedRooms = await reservationRepository.GetReservedRoomsCountAsync(
                    request.FacilityId,
                    roomRequest.RoomType,
                    DateTime.Parse(request.StartDate),
                    DateTime.Parse(request.EndDate));

                var availableRooms = totalRooms - reservedRooms;
                if (availableRooms < roomRequest.Quantity)
                {
                    result.IsAvailable = false;
                    return Result<AvailabilityResultDto>.Failure(
                        new Error($"Not enough {roomRequest.RoomType} rooms available"));
                }
            }

            // check package availability
            if (request.Packages.Any())
            {
                //check if facility has packages
                if (!facility.HasPackages)
                {
                    result.IsAvailable = false;
                    return Result<AvailabilityResultDto>.Failure(new Error("Facility does not have packages"));
                }
            }

            // check if package is available
            foreach (var packageRequest in request.Packages)
            {
                var package = await packageRepository.GetByIdAsync(packageRequest.PackageId, cancellationToken);
                if (package == null || package.FacilityID != request.FacilityId)
                {
                    result.IsAvailable = false;
                    return Result<AvailabilityResultDto>.Failure(new Error("Package not found"));
                }

                // check if package is reserved
                var reservedPackages = await reservationRepository.IsPackageReservedAsync(packageRequest.PackageId);

                if (reservedPackages)
                {
                    result.IsAvailable = false;
                    return Result<AvailabilityResultDto>.Failure(new Error("Package is already reserved"));
                }
            }

            result.IsAvailable = true;
            return Result<AvailabilityResultDto>.Success(result);
        }

    }
}
