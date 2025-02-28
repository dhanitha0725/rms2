using System.Text.RegularExpressions;
using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageFacility.AddRooms
{
    public class AddRoomsCommandHandler(
            IGenericRepository<Facility, int> facilityRepository,
            IGenericRepository<Room, int> roomRepository,
            IUnitOfWork unitOfWork,
            ILogger logger)
            : IRequestHandler<AddRoomsCommand, Result<RoomConfigurationDto>>
    {
        public async Task<Result<RoomConfigurationDto>> Handle(
            AddRoomsCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Check if facility exists
                var facility = await facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
                if (facility == null)
                {
                    return Result<RoomConfigurationDto>.Failure(new Error("Facility not found"));
                }

                var existingRooms = await roomRepository.GetAllAsync(cancellationToken);
                existingRooms = existingRooms.Where(r => r.FacilityID == request.FacilityId);

                var prefix = GetRoomPrefix(facility.FacilityName);
                var maxNumber = GetMaxRoomNumber(existingRooms);
                var roomNumbers = GenerateRoomNumbers(prefix, maxNumber, request.RoomConfigurationDto.Quantity);

                // Create rooms
                var rooms = roomNumbers.Select(rn => new Room
                {
                    FacilityID = request.FacilityId,
                    Type = request.RoomConfigurationDto.RoomType,
                    Capacity = request.RoomConfigurationDto.Capacity,
                    RoomNumber = rn,
                    NumberOfBeds = request.RoomConfigurationDto.NumberOfBeds,
                    Status = request.RoomConfigurationDto.Status
                }).ToList();

                await roomRepository.AddRangeAsync(rooms, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("Rooms added successfully");

                return Result<RoomConfigurationDto>.Success(request.RoomConfigurationDto);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error adding rooms");
                throw;
            }
        }

        private static char GetRoomPrefix(string facilityName)
        {
            var firstLetter = facilityName?.Trim().FirstOrDefault(char.IsLetter);
            return firstLetter.HasValue ? char.ToUpper(firstLetter.Value) : 'R';
        }

        private static int GetMaxRoomNumber(IEnumerable<Room> existingRooms)
        {
            return existingRooms
                .Select(r => int.TryParse(Regex.Match(r.RoomNumber, @"\d+").Value, out var num) ? num : 0)
                .DefaultIfEmpty(0)
                .Max();
        }

        private static IEnumerable<string> GenerateRoomNumbers(
            char prefix,
            int startNumber,
            int quantity)
        {
            for (int i = 1; i <= quantity; i++)
            {
                yield return $"{prefix}{(startNumber + i):D3}";
            }
        }
    }
}
