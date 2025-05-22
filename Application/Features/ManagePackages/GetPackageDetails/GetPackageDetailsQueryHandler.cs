using Application.DTOs.PackageDto;
using Domain.Common;
using Domain.Entities;
using Application.Abstractions.Interfaces;
using MediatR;

namespace Application.Features.ManagePackages.GetPackageDetails
{
    public class GetPackageDetailsQueryHandler(
        IGenericRepository<Package, int> packageRepository,
        IGenericRepository<Pricing, int> pricingRepository,
        IGenericRepository<Facility, int> facilityRepository) 
        : IRequestHandler<GetPackageDetailsQuery, Result<List<PackageTableDetailsDto>>>
    {
        

        public async Task<Result<List<PackageTableDetailsDto>>> Handle(
            GetPackageDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var packages = await packageRepository.GetAllAsync(cancellationToken);
            var pricing = await pricingRepository.GetAllAsync(cancellationToken);
            var facilities = await facilityRepository.GetAllAsync(cancellationToken);

            var packageDetails = packages.Select(pkg =>
            {
                var facilityName = facilities.FirstOrDefault(
                    f => f.FacilityID == pkg.FacilityID)?.FacilityName ?? string.Empty;
                
                // get package pricing
                var packagePricing = pricing
                    .Where(p => p.PackageID == pkg.PackageID)
                    .ToDictionary(p => p.Sector, p => p.Price);

                return new PackageTableDetailsDto
                {
                    PackageId = pkg.PackageID,
                    PackageName = pkg.PackageName,
                    Duration = pkg.Duration,
                    FacilityName = facilityName,
                    Pricings = packagePricing
                };
            }).ToList();

            return Result<List<PackageTableDetailsDto>>.Success(packageDetails);
        }
    }
}
