using Application.DTOs.Payment;
using Domain.Common;
using MediatR;

namespace Application.Features.ManagePayments.CreatePayment
{
    public class CreatePaymentCommand : IRequest<Result<PaymentInitiationResponse>>
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "LKR";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; } = "Sri Lanka";
        public string Items { get; set; }
        public int ReservationId { get; set; }
    }
}
