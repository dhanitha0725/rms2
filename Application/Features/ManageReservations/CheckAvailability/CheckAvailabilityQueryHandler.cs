using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.CheckAvailability
{
    public class CheckAvailabilityQueryHandler(
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
                      rp.Reservation.Status != ReservationStatus.Cancelled &&
                      rp.Reservation.Status != ReservationStatus.Completed &&
                      rp.StartDate < request.EndDate &&
                      rp.EndDate > request.StartDate,
                cancellationToken);

            if (hasOverlap)
            {
                logger.Information("Package with ID {PackageId} has overlapping active reservations between {StartDate} and {EndDate}.",
                    packageId, request.StartDate, request.EndDate);
                return false;
            }

            logger.Information("Package with ID {PackageId} is available between {StartDate} and {EndDate}.",
                packageId, request.StartDate, request.EndDate);
            return true;
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
                         rr.Reservation.Status != ReservationStatus.Cancelled &&
                         rr.Reservation.Status != ReservationStatus.Completed &&
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