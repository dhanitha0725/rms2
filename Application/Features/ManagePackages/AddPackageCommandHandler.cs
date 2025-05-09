using Application.Abstractions.Interfaces;
using Application.DTOs.PackageDto;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManagePackages
{
    public class AddPackageCommandHandler(
            IGenericRepository<Package, int> packageRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger logger)
            : IRequestHandler<AddPackageCommand, Result<AddPackageDto>>
    {
        public async Task<Result<AddPackageDto>> Handle(
            AddPackageCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var package = mapper.Map<Package>(request.PackageDto);
                package.FacilityID = request.FacilityId;

                await packageRepository.AddAsync(package, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<AddPackageDto>.Success(request.PackageDto);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "An error occurred while adding a package");
                throw;
            }
        }
    }
}
