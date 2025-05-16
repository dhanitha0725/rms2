using Application.DTOs.PackageDto;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePackages.AddPackage
{
    public record AddPackageCommand (int FacilityId,AddPackageDto PackageDto): IRequest<Result<AddPackageDto>>
    {
    }
}
