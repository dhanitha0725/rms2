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
                    IGenericRepository<RoomPricing, int> roomPricingRepository,
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
                var rooms = Enumerable
                    .Range(1, request.RoomConfigurationDto.Quantity)
                    .Select(_ => new Room
                    {
                        FacilityID = request.FacilityId,
                        Type = request.RoomConfigurationDto.RoomType,
                        Capacity = request.RoomConfigurationDto.Capacity,
                        NumberOfBeds = request.RoomConfigurationDto.NumberOfBeds,
                        Status = request.RoomConfigurationDto.Status
                    }).ToList();

                await roomRepository.AddRangeAsync(rooms, cancellationToken);

                // add or update pricing for the room type
                foreach (var pricing in request.RoomConfigurationDto.Pricing)
                {
                    var sector = pricing.Key;
                    var price = pricing.Value;

                    //check if pricing exists
                    var existingPricing = (await roomPricingRepository.GetAllAsync(cancellationToken))
                        .FirstOrDefault(rp => rp.FacilityID == request.FacilityId &&
                                              rp.RoomType == request.RoomConfigurationDto.RoomType &&
                                              rp.Sector == sector);

                    if (existingPricing == null)
                    {
                        // add new pricing
                        var roomPricing = new RoomPricing
                        {
                            FacilityID = request.FacilityId,
                            RoomType = request.RoomConfigurationDto.RoomType,
                            Sector = sector,
                            Price = price
                        };

                        await roomPricingRepository.AddAsync(roomPricing, cancellationToken);
                    }
                    else
                    {
                        // update existing pricing
                        existingPricing.Price = price;
                        await roomPricingRepository.UpdateAsync(existingPricing, cancellationToken);
                    }
                }
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("Rooms and pricing added successfully");

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
