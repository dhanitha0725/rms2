using Application.DTOs.Payment;
using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.ApproveDocument
{
    public class ApproveDocumentCommand : IRequest<Result<PaymentInitiationResponse>>
    {
        public int DocumentId { get; set; }
        public string DocumentType { get; set; }
        public decimal? AmountPaid { get; set; }
        public bool IsApproved { get; set; }
        public string? OrderId { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; } = "LKR";
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Items { get; set; }
   
    }
}
