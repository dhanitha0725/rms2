using Application.DTOs.PackageDto;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePackages
{
    public record AddPackageCommand (int FacilityId,AddPackageDto PackageDto): IRequest<Result<AddPackageDto>>
    {
    }
}
