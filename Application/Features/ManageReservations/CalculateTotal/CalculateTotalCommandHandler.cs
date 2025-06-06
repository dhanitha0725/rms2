﻿using Application.Abstractions.Interfaces;
using Application.DTOs.ReservationDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.CalculateTotal
{
    public class CalculateTotalCommandHandler(
            IGenericRepository<Package, int> packageRepository,
            IGenericRepository<Room, int> roomRepository,
            IGenericRepository<Pricing, int> pricingRepository,
            IRoomRepository roomTypeRepository,
            ILogger logger)
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

                decimal subTotal;
                string pricingType;

                // check if package is daily-based (duration >= 24 hours)
                if (package.Duration is { TotalHours: >= 24 })
                {
                    if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                        return Result<PriceBreakdownDto>.Failure(new Error("Dates are required for daily packages"));

                    // Calculate days, handling same-day reservations as 1 day for packages
                    int days;
                    if (request.StartDate.Value.Date == request.EndDate.Value.Date)
                    {
                        // Same day reservation counts as 1 day for packages
                        days = 1;
                        logger.Information("Same-day package reservation detected. Counting as 1 day.");
                    }
                    else
                    {
                        // For multi-day reservations, calculate the difference in days
                        days = (request.EndDate.Value.Date - request.StartDate.Value.Date).Days;

                        // Add 1 to include the end date 
                        days += 1;
                    }

                    subTotal = pricing.First().Price * days;
                    pricingType = "daily";

                    logger.Information("Package {PackageId} duration calculated as {Days} days from {StartDate} to {EndDate}",
                        item.ItemId, days, request.StartDate.Value.ToString("yyyy-MM-dd"),
                        request.EndDate.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    // Fixed duration package
                    subTotal = pricing.First().Price;
                    pricingType = "fixed";
                }

                return Result<PriceBreakdownDto>.Success(new PriceBreakdownDto
                {
                    ItemName = package.PackageName,
                    PricingType = pricingType,
                    UnitPrice = pricing.First().Price,
                    Quantity = item.Quantity,
                    SubTotal = subTotal * item.Quantity,
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error processing package {PackageId}", item.ItemId);
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
                // get room details
                var room = await roomRepository.GetByIdAsync(item.ItemId, cancellationToken);
                if (room == null)
                    return Result<PriceBreakdownDto>.Failure(new Error("Room not found"));

                // get room pricing with room type included
                var roomPricingList = await roomTypeRepository.GetRoomPricingWithRoomTypeAsync(
                    request.FacilityId, new List<int> { room.RoomTypeID }, cancellationToken);

                // filter pricing based on customer type
                var matchingPricing = roomPricingList
                    .Where(rp => rp.RoomTypeID == room.RoomTypeID && rp.Sector == request.CustomerType)
                    .ToList();

                // check if any pricing is available
                if (!matchingPricing.Any())
                    return Result<PriceBreakdownDto>.Failure(new Error("No pricing available"));

                var duration = (request.EndDate.Value - request.StartDate.Value).Days;
                var pricePerNight = matchingPricing.First().Price;
                var roomTypeName = matchingPricing.First().RoomType.TypeName;

                return Result<PriceBreakdownDto>.Success(new PriceBreakdownDto
                {
                    ItemName = room.RoomType.TypeName,
                    PricingType = "daily",
                    UnitPrice = pricePerNight,
                    Quantity = item.Quantity,
                    SubTotal = pricePerNight * item.Quantity * duration,
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return Result<PriceBreakdownDto>.Failure(new Error(ex.Message));
            }
        }
    }
}
