using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageFacility.SelectedFacilityDetails
{
    public class GetSelectedFacilityDetailsQueryHandler(
                IGenericRepository<Image, int> imageRepository,
                IGenericRepository<Facility, int> facilityRepository,
                IGenericRepository<Package, int> packageRepository,
                IGenericRepository<Pricing, int> pricingRepository,
                IGenericRepository<Room, int> roomRepository,
                IGenericRepository<RoomPricing, int> roomPricingRepository,
                IRoomRepository roomDetailsRepository)
                : IRequestHandler<GetSelectedFacilityDetailsQuery, Result<SelectedFacilityDetailsDto>>
    {
        public async Task<Result<SelectedFacilityDetailsDto>> Handle(
            GetSelectedFacilityDetailsQuery request,
            CancellationToken cancellationToken)
        {

            // Get facility by ID
            var facility = await facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);

            if (facility == null)
            {
                return Result<SelectedFacilityDetailsDto>.Failure(new Error("Facility not found"));
            }

            // Get images
            var images = await imageRepository.GetAllAsync(cancellationToken);
            var imageUrls = images
                .Where(i => i.FacilityID == request.FacilityId)
                .Select(i => i.ImageUrl)
                .ToList();

            // Get facility attributes
            var facilityAttributes = facility.Attributes ?? new List<string>();

            // Get packages with pricing
            var packages = (await packageRepository.GetAllAsync(cancellationToken))
                .Where(p => p.FacilityID == request.FacilityId)
                .ToList();

            var packageIds = packages.Select(p => p.PackageID).ToList();
            var pricing = (await pricingRepository.GetAllAsync(cancellationToken))
                .Where(p => packageIds.Contains(p.PackageID))
                .GroupBy(p => p.PackageID)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Get rooms
            var rooms = (await roomRepository.GetAllAsync(cancellationToken))
                .Where(r => r.FacilityID == request.FacilityId)
                .ToList();

            // Get room types
            var roomTypes = rooms.Select(r => r.RoomTypeID).Distinct().ToList();

            // Fetch room pricing with room types
            var roomPricingList = await roomDetailsRepository.GetRoomPricingWithRoomTypeAsync(
                request.FacilityId, roomTypes, cancellationToken);
            var roomPricing = roomPricingList
                .GroupBy(rp => rp.RoomType)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Build the result
            return Result<SelectedFacilityDetailsDto>.Success(new SelectedFacilityDetailsDto
            {
                // Get facility details
                FacilityId = facility.FacilityID,
                FacilityName = facility.FacilityName,
                Location = facility.Location,
                Description = facility.Description,
                Attributes = facilityAttributes,
                ImageUrls = imageUrls,

                // Get package details
                Packages = packages.Select(p => new PackageDetailsDto
                {
                    PackageId = p.PackageID,
                    PackageName = p.PackageName,
                    Duration = p.Duration,
                    Pricing = pricing.GetValueOrDefault(p.PackageID, new List<Pricing>())
                        .Select(pr => new PricingDto { Sector = pr.Sector, Price = pr.Price })
                        .ToList()
                }).ToList(),

                // Get room details
                Rooms = roomPricing.Select(kvp => new RoomDetailsDto
                {
                    RoomTypeId = kvp.Key.RoomTypeID,
                    RoomType = kvp.Key.TypeName,
                    RoomPricing = kvp.Value
                        .Select(rp => new RoomPricingDto
                        {
                            Sector = rp.Sector,
                            Price = rp.Price
                        }).ToList()
                }).ToList()
            });
        }
    }
}
