using System.Text.Json;
using MediatR;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Serilog;

namespace Application.Features.ManageFacility.AddFacility.Commands
{
    public class AddFacilityCommandHandler (
        IGenericRepository<Facility, int> facilityRepository,
        IGenericRepository<FacilityType, int> facilityTypeRepository,
        IGenericRepository<Image, int> imageRepository,
        IGoogleDriveService googleDriveService,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ILogger logger) 
        : IRequestHandler<AddFacilityCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            AddFacilityCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                //check facility type exists
                var typeExists = await facilityTypeRepository.ExistsAsync(
                    ft => ft.FacilityTypeId == request.FacilityDto.FacilityTypeId,
                    cancellationToken);

                if (!typeExists)
                {
                    return Result<int>.Failure(new Error("Invalid facility type"));
                }

                // Create facility
                var facility = mapper.Map<Facility>(request.FacilityDto);
                facility.FacilityTypeId = request.FacilityDto.FacilityTypeId;
                facility.CreatedDate = DateTime.UtcNow;

                // Ensure attributes are properly serialized before storing
                if (request.FacilityDto.Attributes != null)
                {
                    facility.Attributes = JsonSerializer
                        .Serialize(request.FacilityDto.Attributes);
                }

                await facilityRepository.AddAsync(facility, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                // Handle image upload only if images exist
                //if (request.FacilityDto.Images != null && request.FacilityDto.Images?.Count > 0)
                //{
                //    try
                //    {
                //        var imageUrls = await googleDriveService.UploadImagesAsync(request.FacilityDto.Images);

                //        // Create and save image entities
                //        var images = imageUrls.Select(url => new Image
                //        {
                //            FacilityID = facility.FacilityID,
                //            ImageUrl = url
                //        }).ToList();

                //        foreach (var image in images)
                //        {
                //            await imageRepository.AddAsync(image, cancellationToken);
                //        }

                //        await unitOfWork.SaveChangesAsync(cancellationToken);
                //    }
                //    catch (Exception ex)
                //    {
                //        logger.Error(ex, "Error uploading images for facility {FacilityId}", facility.FacilityID);
                //        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                //        throw;
                //    }
                //}

                logger.Information("New Facility Added with Id: {FacilityId}", facility.FacilityID);
                return Result<int>.Success(facility.FacilityID);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(ex, "Error adding new facility");
                throw;
            }
        }
    }
}
