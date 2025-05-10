using Application.DTOs.ReservationDtos;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.UploadDocument
{
    public class UploadDocumentsCommand : IRequest<Result<int>>
    {
        public Guid PaymentId { get; set; }
        public int ReservationId { get; set; }
        public DocumentDto Document { get; set; }
    }
}
