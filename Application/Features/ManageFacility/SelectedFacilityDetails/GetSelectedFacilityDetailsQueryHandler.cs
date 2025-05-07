using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;
using System.Text.Json;

namespace Application.Features.ManageFacility.SelectedFacilityDetails
{
    public class GetSelectedFacilityDetailsQueryHandler(
                IGenericRepository<Image, int> imageRepository,
                IGenericRepository<Facility, int> facilityRepository,
                IGenericRepository<Package, int> packageRepository,
                IGenericRepository<Pricing, int> pricingRepository,
                IGenericRepository<Room, int> roomRepository,
                IGenericRepository<RoomPricing, int> roomPricingRepository)
                : IRequestHandler<GetSelectedFacilityDetailsQuery, Result<SelectedFacilityDetailsDto>>
    {
        public async Task<Result<SelectedFacilityDetailsDto>> Handle(
            GetSelectedFacilityDetailsQuery request,
            CancellationToken cancellationToken)
        {
            // get facility by id
            var facility = await facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);

            if (facility == null)
            {
                return Result<SelectedFacilityDetailsDto>.Failure(new Error("Facility not found"));
            }

            // get images
            var images = await imageRepository.GetAllAsync(cancellationToken);
            var imageUrls = images
                .Where(i => i.FacilityID == request.FacilityId)
                .Select(i => i.ImageUrl)
                .ToList();

            // get facility attributes
            var facilityAttributes = facility.Attributes ?? new List<string>();

            // get packages with pricing
            var packages = (await packageRepository.GetAllAsync(cancellationToken))
                .Where(p => p.FacilityID == request.FacilityId)
                .ToList();

            var packageIds = packages.Select(p => p.PackageID).ToList();
            var pricing = (await pricingRepository.GetAllAsync(cancellationToken))
                .Where(p => packageIds.Contains(p.PackageID))
                .GroupBy(p => p.PackageID)
                .ToDictionary(g => g.Key, g => g.ToList());

            // get rooms with pricing
            var rooms = (await roomRepository.GetAllAsync(cancellationToken))
                .Where(r => r.FacilityID == request.FacilityId)
                .ToList();
            var roomTypes = rooms.Select(r => r.RoomTypeID).Distinct().ToList();
            var roomPricing = (await roomPricingRepository.GetAllAsync(cancellationToken))
                .Where(rp => rp.FacilityID == request.FacilityId && roomTypes.Contains(rp.RoomTypeID))
                .GroupBy(rp => rp.RoomType)
                .ToDictionary(g => g.Key, g => g.ToList());

            return Result<SelectedFacilityDetailsDto>.Success(new SelectedFacilityDetailsDto
            {
                FacilityId = facility.FacilityID,
                FacilityName = facility.FacilityName,
                Location = facility.Location,
                Description = facility.Description,
                Attributes = facilityAttributes,
                ImageUrls = imageUrls,

                Packages = packages.Select(p => new PackageDetailsDto
                {
                    PackageId = p.PackageID,
                    PackageName = p.PackageName,
                    Duration = p.Duration,
                    Pricing = pricing.GetValueOrDefault(p.PackageID, new List<Pricing>())
                        .Select(pr => new PricingDto { Sector = pr.Sector, Price = pr.Price })
                        .ToList()
                }).ToList(),
                Rooms = rooms.Select(r => new RoomDetailsDto
                {
                    RoomId = r.RoomID,
                    RoomType = r.RoomType.TypeName,
                    RoomPricing = roomPricing.GetValueOrDefault(r.RoomType, new List<RoomPricing>())
                        .Select(rp => new RoomPricingDto { Sector = rp.Sector, Price = rp.Price })
                        .ToList()
                }).ToList()
            });
        }
    }
}
