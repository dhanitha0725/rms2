using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageReservations.AddReservationByCustomer.UploadDocuments
{
    public class UploadDocumentsCommandHandler(
                IGoogleDriveService driveService,
                IGenericRepository<Document, int> documentRepository,
                IUnitOfWork unitOfWork,
                ILogger logger)
                : IRequestHandler<UploadDocumentsCommand, Result>
    {
        public async Task<Result> Handle(
            UploadDocumentsCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var urls = await driveService.UploadFilesAsync(request.Documents);
                foreach (var url in urls)
                {
                    var document = new Document
                    {
                        ReservationID = request.ReservationID,
                        UserID = request.UserID,
                        Url = url,
                        DocumentType = "ReservationDocument"
                    };

                    await documentRepository.AddAsync(document, cancellationToken);
                }
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.Information("Documents uploaded successfully");
                return Result.Success([]);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error uploading documents");
                throw;
            }

        }
    }
}
