using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManagePackages.AddRoomPricing
{
    public class AddRoomPricingCommandHandler(
        IGenericRepository<RoomPricing, int> roomPricingRepository,
        IGenericRepository<Facility, int> facilityRepository,
        IGenericRepository<RoomType, int> roomTypeRepository,
        IUnitOfWork unitOfWork,
        ILogger logger)
        : IRequestHandler<AddRoomPricingCommand, Result>
    {
        public async Task<Result> Handle(
            AddRoomPricingCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Check if facility exists
                var facility = await facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
                if (facility == null)
                {
                    return Result.Failure(new Error($"Facility with ID {request.FacilityId} not found."));
                }

                // Check if room type exists
                var roomType = await roomTypeRepository.GetByIdAsync(request.RoomTypeId, cancellationToken);
                if (roomType == null)
                {
                    return Result.Failure(new Error($"Room type with ID {request.RoomTypeId} not found."));
                }

                // Get all existing pricing for this facility and room type
                var existingPricings = (await roomPricingRepository.GetAllAsync(cancellationToken))
                    .Where(rp => rp.FacilityID == request.FacilityId && rp.RoomTypeID == request.RoomTypeId)
                    .ToList();

                foreach (var sectorPrice in request.Pricings)
                {
                    var sector = sectorPrice.Key;
                    var price = sectorPrice.Value;

                    var existing = existingPricings.FirstOrDefault(rp => rp.Sector == sector);

                    if (existing != null)
                    {
                        // Update existing price
                        existing.Price = price;
                        await roomPricingRepository.UpdateAsync(existing, cancellationToken);
                    }
                    else
                    {
                        // Add new price
                        var newPricing = new RoomPricing
                        {
                            FacilityID = request.FacilityId,
                            RoomTypeID = request.RoomTypeId,
                            Sector = sector,
                            Price = price
                        };
                        await roomPricingRepository.AddAsync(newPricing, cancellationToken);
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("Room pricing added/updated for FacilityId={FacilityId}, RoomTypeId={RoomTypeId}",
                    request.FacilityId, request.RoomTypeId);
                return Result.Success([]);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(ex, "Error adding/updating room pricing");
                return Result.Failure(new Error("Failed to add/update room pricing."));
            }
        }
    }
}
