using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ManageReservations.AddReservationByCustomer.ProcessPayment
{
    public class ProcessPaymentCommand : IRequest<Result>
    {
        public int ReservationId { get; set; }
        public string PaymentMethod { get; set; } 
        public List<IFormFile>? BankTransferReceipts { get; set; }
        public decimal AmountPaid { get; set; }
        public int UserId { get; set; }
    }
}
