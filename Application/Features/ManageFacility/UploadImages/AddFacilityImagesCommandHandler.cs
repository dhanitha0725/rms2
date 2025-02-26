using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageFacility.UploadImages
{
    public class AddFacilityImagesCommandHandler (
        IGenericRepository<Image, int> imageRepository,
        IGenericRepository<Facility, int> facilityRepository,
        IGoogleDriveService driveService,
        IUnitOfWork unitOfWork,
        ILogger logger)
        : IRequestHandler<AddFacilityImagesCommand, Result<List<string>>>
    {
        public async Task<Result<List<string>>> Handle(
            AddFacilityImagesCommand request, 
            CancellationToken cancellationToken)
        {
            try
            {
                var facilityExists = await facilityRepository.ExistsAsync(
                    f => f.FacilityID == request.FacilityId, 
                    cancellationToken);

                if (!facilityExists)
                {
                    return Result<List<string>>.Failure(new Error("Facility not found"));
                }

                //upload images
                var imageUrls = await driveService.UploadImagesAsync(
                    request.AddFacilityImagesDto.Images);

                //save to db
                var images = imageUrls.Select(url => new Image
                {
                    FacilityID = request.FacilityId,
                    ImageUrl = url
                }).ToList();

                await imageRepository.AddRangeAsync(images, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<List<string>>.Success(imageUrls);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error uploading images");
                throw;
            }
        }
    }
}
