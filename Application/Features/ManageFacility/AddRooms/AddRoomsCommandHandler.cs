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

                // Create rooms
                var rooms = Enumerable.Range(1, request.RoomConfigurationDto.Quantity).Select(_ => new Room
                {
                    FacilityID = request.FacilityId,
                    Type = request.RoomConfigurationDto.RoomType,
                    Capacity = request.RoomConfigurationDto.Capacity,
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
    }
}
