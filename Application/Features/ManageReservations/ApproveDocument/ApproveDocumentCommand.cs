using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.ApproveDocument
{
    public class ApproveDocumentCommand : IRequest<Result>
    {
        public int DocumentId { get; set; }
        public string DocumentType { get; set; }
        public decimal? AmountPaid { get; set; }
        public bool IsApproved { get; set; }
    }
}
