using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Features.ManageReservations.UploadDocument
{
    public class UploadDocumentsCommandHandler(
            IGenericRepository<Document, int> documentRepository,
            IGenericRepository<Reservation, int> reservationRepository,
            IGoogleDriveService googleDriveService,
            IUnitOfWork unitOfWork,
            ILogger logger)
            : IRequestHandler<UploadDocumentsCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            UploadDocumentsCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // fetch the reservation
                var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
                if (reservation == null)
                {
                    logger.Information("Reservation Not Found");
                    return Result<int>.Failure(new Error("Reservation not found"));
                }

                // validate reservation status
                if (reservation.Status != ReservationStatus.PendingApproval)
                {
                    logger.Information("invalid document upload status");
                    return Result<int>.Failure(new Error("Invalid document upload status"));
                }

                // upload the document
                var fileUrl = await googleDriveService.UploadFilesAsync(new List<IFormFile> { request.Document.File });
                if (!fileUrl.Any())
                {
                    logger.Information("file upload failed");
                    return Result<int>.Failure(new Error("Failed to upload document"));
                }

                var document = new Document
                {
                    DocumentType = request.Document.DocumentType,
                    Url = fileUrl.First(),
                    ReservationId = reservation.ReservationID,
                };

                await documentRepository.AddAsync(document, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("file upload success");
                return Result<int>.Success(document.DocumentID);
            }
            catch (Exception e)
            {
                logger.Error("Reservation failed {message}", e.Message);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
