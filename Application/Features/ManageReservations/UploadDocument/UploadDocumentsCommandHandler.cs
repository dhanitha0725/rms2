using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Application.Features.ManageReservations.UploadDocument
{
    public class UploadDocumentsCommandHandler(
                IGenericRepository<Document, int> documentRepository,
                IGenericRepository<Reservation, int> reservationRepository,
                IGenericRepository<Payment, Guid> paymentRepository,
                IBlobService blobService,
                IUnitOfWork unitOfWork,
                ILogger logger,
                IConfiguration configuration)
                : IRequestHandler<UploadDocumentsCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            UploadDocumentsCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Get container name from configuration
                var containerName = configuration["BlobContainerSettings:ContainerName"];
                switch (request.Document.DocumentType)
                {
                    // Validate based on DocumentType
                    case nameof(DocumentType.ApprovalDocument):
                    {
                        var reservation = await reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
                        if (reservation == null)
                        {
                            logger.Warning("Reservation not found for ID: {ReservationId}", request.ReservationId);
                            return Result<int>.Failure(new Error("Reservation not found"));
                        }

                        break;
                    }
                    case nameof(DocumentType.BankReceipt):
                    {
                        var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
                        if (payment == null)
                        {
                            logger.Warning("Payment not found for ID: {PaymentId}", request.PaymentId);
                            return Result<int>.Failure(new Error("Payment not found"));
                        }

                        break;
                    }
                }

                // Upload the document to blob storage
                var fileUrl = await blobService.UploadFileAsync(request.Document.File, containerName);
                if (string.IsNullOrEmpty(fileUrl))
                {
                    logger.Information("File upload failed");
                    return Result<int>.Failure(new Error("Failed to upload document"));
                }

                // Create the document entity
                var document = new Document
                {
                    DocumentType = request.Document.DocumentType,
                    Url = fileUrl,
                    ReservationId = request.ReservationId != 0 ? request.ReservationId : null,
                    PaymentId = request.PaymentId != Guid.Empty ? request.PaymentId : null
                };

                await documentRepository.AddAsync(document, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                logger.Information("File upload success");
                return Result<int>.Success(document.DocumentID);
            }
            catch (Exception e)
            {
                logger.Error("Document upload failed: {Message}", e.Message);
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
