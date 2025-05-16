using Application.DTOs.FacilityDtos;
using Application.DTOs.PackageDto;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePackages.GetPackageDetails
{
    public class GetPackageDetailsQuery : IRequest<Result<List<PackageTableDetailsDto>>>
    {
    }
}
