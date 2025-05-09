using Application.Abstractions.Interfaces;
using Application.DTOs.FacilityDtos;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageFacility.FacilityCardDetails
{
    public class GetFacilityCardsQueryHandler(
        IGenericRepository<Facility, int> facilityRepository,
        IGenericRepository<Image, int> imageRepository,
        IGenericRepository<Package, int> packageRepository,
        IGenericRepository<Pricing, int> pricingRepository,
        IGenericRepository<RoomPricing, int> roomPricingRepository) 
        : IRequestHandler<GetFacilityCardsQuery, Result<List<FacilityCardDto>>>
    {
        public async Task<Result<List<FacilityCardDto>>> Handle(
            GetFacilityCardsQuery request, 
            CancellationToken cancellationToken)
        {
            // get all facilities
            var facilities = await facilityRepository.GetAllAsync(cancellationToken);

            var facilityIds = facilities.Select(f => f.FacilityID).ToList();

            // get all data that used in FacilityCardDto
            var images = await imageRepository.GetAllAsync(cancellationToken);
            var packages = await packageRepository.GetAllAsync(cancellationToken);
            var pricings = await pricingRepository.GetAllAsync(cancellationToken);
            var roomPricings = await roomPricingRepository.GetAllAsync(cancellationToken);

            var facilityDtos = facilities.Select(facility =>
            {
                var facilityImages = images
                    .Where(i => i.FacilityID == facility.FacilityID);
                var facilityPackages = packages
                    .Where(p => p.FacilityID == facility.FacilityID);
                var packageIds = facilityPackages
                    .Select(fp => fp.PackageID).ToList();
                var facilityPackagePricings = pricings
                    .Where(p => facilityPackages.Any(fp => fp.PackageID == p.PackageID));
                var facilityRoomPricings = roomPricings
                    .Where(rp => rp.FacilityID == facility.FacilityID);

                return new FacilityCardDto
                {
                    FacilityID = facility.FacilityID,
                    FacilityName = facility.FacilityName,
                    Location = facility.Location,
                    ImageUrl = facilityImages.FirstOrDefault()?.ImageUrl,
                    Price = GetMinPrice(facilityPackagePricings, facilityRoomPricings)
                };

            }).ToList();

            return Result<List<FacilityCardDto>>.Success(facilityDtos);
        }

        private decimal GetMinPrice(
            IEnumerable<Pricing> packagePricings,
            IEnumerable<RoomPricing> roomPricings)
        {
            var prices = new List<decimal>();

            if (packagePricings.Any())
            {
                prices.Add(packagePricings.Min(p => p.Price));
            }

            if (roomPricings.Any())
            {
                prices.Add(roomPricings.Min(rp => rp.Price));
            }

            return prices.Any() ? prices.Min() : 0;
        }
    }
}
