using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageReservations.CalculateTotal
{
    public class CalculateTotalCommandHandler(
            IGenericRepository<Package, int> packageRepository,
            IGenericRepository<Room, int> roomRepository,
            IGenericRepository<Pricing, int> pricingRepository,
            IGenericRepository<RoomPricing, int> roomPricingRepository)
            : IRequestHandler<CalculateTotalCommand, Result<CalculateTotalResponseDto>>
    {
        public async Task<Result<CalculateTotalResponseDto>> Handle(
            CalculateTotalCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // check date range is needed based on the selected item
                var requireDates = false;
                foreach (var i in request.CalculateTotalDto.SelectedItems)
                {
                    if (i.Type == "room" || (await IsDailyPackage(i.ItemId)))
                    {
                        requireDates = true;
                        break;
                    }
                }

                // validate dates
                if (requireDates)
                {
                    var (valid, error) = ValidateDates(request.CalculateTotalDto);
                    if (!valid)
                    {
                        return Result<CalculateTotalResponseDto>.Failure(error);
                    }
                }

                var processingResults = new List<Result<PriceBreakdownDto>>();

                // process each selected item (package or room)
                // and calculate the total price
                foreach (var item in request.CalculateTotalDto.SelectedItems)
                {
                    Result<PriceBreakdownDto> result;

                    if (item.Type == "package")
                    {
                        // process package 
                        result = await ProcessPackage(item, request.CalculateTotalDto, cancellationToken);
                    }
                    else if (item.Type == "room")
                    {
                        // process room 
                        result = await ProcessRoom(item, request.CalculateTotalDto, cancellationToken);
                    }
                    else
                    {
                        result = Result<PriceBreakdownDto>.Failure(new Error("Invalid item type"));
                    }

                    processingResults.Add(result);
                }

                // Check if any processing failed and return errors if necessary
                if (processingResults.Any(r => !r.IsSuccess))
                {
                    var errorMessages = processingResults
                        .Where(r => !r.IsSuccess)
                        .Select(r => r.Error.Message)
                        .ToList();

                    return Result<CalculateTotalResponseDto>.Failure(new Error(string.Join("; ", errorMessages)));
                }

                // Calculate the total and prepare the breakdown of prices
                var breakdown = processingResults
                    .Where(r => r.IsSuccess)
                    .Select(r => r.Value)
                    .ToList();

                var total = breakdown.Sum(b => b.SubTotal);

                // response with the total and breakdown (detailed pricing details)
                var response = new CalculateTotalResponseDto
                {
                    Total = total,
                    Breakdown = breakdown
                };

                return Result<CalculateTotalResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return Result<CalculateTotalResponseDto>.Failure(new Error(ex.Message));
            }
        }

        // check if a package is a daily package based on its duration
        private async Task<bool> IsDailyPackage(int packageId)
        {
            var package = await packageRepository.GetByIdAsync(packageId);
            return package?.Duration >= TimeSpan.FromHours(24);
        }

        // validate the start and end dates
        private (bool valid, Error error) ValidateDates(CalculateTotalDto request)
        {
            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                return (false, new Error("Please select the start and end dates."));

            if (request.StartDate.Value >= request.EndDate.Value)
                return (false, new Error("End date must be after start date"));

            return (true, null)!;
        }

        // process a package and calculate its price breakdown
        private async Task<Result<PriceBreakdownDto>> ProcessPackage(
            BookingItemDto item,
            CalculateTotalDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var package = await packageRepository.GetByIdAsync(item.ItemId, cancellationToken);
                if (package == null)
                    return Result<PriceBreakdownDto>.Failure(new Error("Package not found"));

                var pricing = (await pricingRepository.GetAllAsync(cancellationToken))
                    .Where(p => p.PackageID == package.PackageID && p.Sector == request.CustomerType);

                if (!pricing.Any()) return Result<PriceBreakdownDto>.Failure(new Error("No pricing available"));

                return Result<PriceBreakdownDto>.Success(new PriceBreakdownDto
                {
                    ItemName = package.PackageName,
                    PricingType = "fixed",
                    UnitPrice = pricing.First().Price,
                    Quantity = item.Quantity,
                    SubTotal = pricing.First().Price * item.Quantity,
                });
            }
            catch (Exception ex)
            {
                return Result<PriceBreakdownDto>.Failure(new Error(ex.Message));
            }
        }

        // Process a room and calculate its price breakdown
        private async Task<Result<PriceBreakdownDto>> ProcessRoom(
            BookingItemDto item,
            CalculateTotalDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var room = await roomRepository.GetByIdAsync(item.ItemId, cancellationToken);

                if (room == null)
                    return Result<PriceBreakdownDto>.Failure(new Error("Room not found"));

                var pricing = (await roomPricingRepository.GetAllAsync(cancellationToken))
                    .Where(rp => rp.RoomType == room.Type && rp.Sector == request.CustomerType);

                if (!pricing.Any())
                    return Result<PriceBreakdownDto>.Failure(new Error("No pricing available"));

                var duration = (request.EndDate.Value - request.StartDate.Value).Days;

                return Result<PriceBreakdownDto>.Success(new PriceBreakdownDto
                {
                    ItemName = room.Type,
                    PricingType = "daily",
                    UnitPrice = pricing.First().Price,
                    Quantity = item.Quantity,
                    SubTotal = pricing.First().Price * item.Quantity * duration,
                });
            }
            catch (Exception ex)
            {
                return Result<PriceBreakdownDto>.Failure(new Error(ex.Message));
            }
        }
    }
}
