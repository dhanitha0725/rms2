using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.CheckAvailability
{
    public class CheckAvailabilityQueryHandler(
            IGenericRepository<ReservedRoom, int> reservedRoomRepository,
            IGenericRepository<ReservedPackage, int> reservedPackageRepository,
            IGenericRepository<Room, int> roomRepository,
            IGenericRepository<Package, int> packageRepository,
            ILogger logger)
            : IRequestHandler<CheckAvailabilityQuery, Result<AvailabilityResponseDto>>
    {
        public async Task<Result<AvailabilityResponseDto>> Handle(
            CheckAvailabilityQuery request,
            CancellationToken cancellationToken) 
        {
            var isAvailable = true;
            var requestDateRange = request.CheckAvailabilityDto;

            foreach (var item in requestDateRange.Items)
            {
                if (item.Type == "package")
                {
                    var result = await CheckPackageAvailability(item, requestDateRange, cancellationToken); 
                    if (!result) isAvailable = false;
                }
                else if (item.Type == "room")
                {
                    var result = await CheckRoomAvailability(item, requestDateRange, cancellationToken); 
                    if (!result) isAvailable = false;
                }
                else
                {
                    logger.Warning("Unknown item type: {ItemType}", item.Type);
                }
            }

            var message = isAvailable
                ? "The facility is available."
                : "The facility is not available.";

            return Result<AvailabilityResponseDto>.Success(new AvailabilityResponseDto
            {
                IsAvailable = isAvailable,
                Message = message
            });
        }

        private async Task<bool> CheckPackageAvailability(
            AvailableItemDto item,
            CheckAvailabilityDto request,
            CancellationToken cancellationToken) 
        {
            var packageId = item.ItemId;

            var package = await packageRepository.GetByIdAsync(packageId, cancellationToken); 
            if (package == null)
            {
                logger.Warning("Package with ID {PackageId} not found.", packageId);
                return false;
            }

            var hasOverlap = await reservedPackageRepository.AnyAsync(
                rp => rp.PackageID == packageId &&
                      rp.StartDate < request.EndDate &&
                      rp.EndDate > request.StartDate,
                cancellationToken);
            if (!hasOverlap) return !hasOverlap;
            logger.Warning("Package with ID {PackageId} has overlapping reservations.", packageId);
            return false;
        }

        private async Task<bool> CheckRoomAvailability(
            AvailableItemDto item,
            CheckAvailabilityDto request,
            CancellationToken cancellationToken)
        {
            var roomTypeId = item.ItemId;
            var facilityId = request.FacilityId;

            // Fetch rooms that are available and not reserved during the requested date range
            var availableRoomsCount = await roomRepository.CountAsync(
                r => r.RoomTypeID == roomTypeId &&
                     r.FacilityID == facilityId &&
                     r.Status == "Available" &&
                     !r.ReservedRooms.Any(rr =>
                         rr.StartDate < request.EndDate &&
                         rr.EndDate > request.StartDate),
                cancellationToken);

            if (availableRoomsCount < item.Quantity)
            {
                logger.Warning("Insufficient rooms of type {RoomTypeId} in facility {FacilityId}.",
                    roomTypeId, facilityId);
                return false;
            }

            return true;
        }
    }
}