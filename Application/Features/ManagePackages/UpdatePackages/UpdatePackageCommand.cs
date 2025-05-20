using System;
using Application.DTOs.PackageDto;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePackages.UpdatePackages;

public class UpdatePackageCommand : IRequest<Result<UpdatePackageDto>>
{
    public int PackageId { get; set; }
    public UpdatePackageDto PackageDto { get; set; }
}
