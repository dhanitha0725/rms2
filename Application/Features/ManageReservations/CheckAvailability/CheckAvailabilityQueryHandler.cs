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
                    var result = await CheckPackageAvailability(item, requestDateRange);
                    if (!result) isAvailable = false;
                }
                else if (item.Type == "room")
                {
                    var result = await CheckRoomAvailability(item, requestDateRange);
                    if (!result) isAvailable = false;
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
            CheckAvailabilityDto request)
        {
            //get package details
            var package = await packageRepository.GetByIdAsync(item.ItemId);
            if (package == null)
            {
                return false;
            }

            //check existing reservations
            var existingReservations = (await reservedPackageRepository.GetAllAsync())
                .Count(rp => rp.PackageID == item.ItemId && 
                             rp.Reservation != null && 
                             rp.Reservation.StartDate < request.EndDate && 
                             rp.Reservation.EndDate > request.StartDate);

            return existingReservations == 0;
        }

        private async Task<bool> CheckRoomAvailability(
            AvailableItemDto item,
            CheckAvailabilityDto request)
        {
            //get room details
            var room = await roomRepository.GetByIdAsync(item.ItemId);
            if (room == null)
            {
                return false;
            }

            //get total room of given type
            var totalRooms = (await roomRepository.GetAllAsync())
                .Count(r => r.Type == room.Type && r.Status == "Available");

            // get total reserved rooms of given type
            var reservedRooms = (await reservedRoomRepository.GetAllAsync())
                .Count(r => r.RoomID == item.ItemId &&
                            r.Reservation.StartDate < request.EndDate &&
                            r.Reservation.EndDate > request.StartDate);

            var available = totalRooms - reservedRooms;
            return available >= item.Quantity;
        }
    }
}