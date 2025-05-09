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
        IBlobService blobService,
        IUnitOfWork unitOfWork,
        ILogger logger)
        : IRequestHandler<AddFacilityImagesCommand, Result<List<string>>>
    {
        public async Task<Result<List<string>>> Handle(
            AddFacilityImagesCommand request, 
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var facilityExists = await facilityRepository.ExistsAsync(
                    f => f.FacilityID == request.FacilityId, 
                    cancellationToken);

                if (!facilityExists)
                {
                    return Result<List<string>>.Failure(new Error("Facility not found"));
                }

                var imageUrls = new List<string>();

                foreach (var file in request.Files)
                {
                    var blobName = await blobService.UploadFileAsync(file, request.ContainerName);
                    var url = await blobService.GetFileUrl(blobName, request.ContainerName);
                    imageUrls.Add(url);
                }

                var images = imageUrls.Select(url => new Image
                {
                    FacilityID = request.FacilityId,
                    ImageUrl = url,
                }).ToList();


                await imageRepository.AddRangeAsync(images, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<List<string>>.Success(imageUrls);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error uploading images");
                return Result<List<string>>.Failure(new Error("Error uploading images"));
            }
        }
    }
}
