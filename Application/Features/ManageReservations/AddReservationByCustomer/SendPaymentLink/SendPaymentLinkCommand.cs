using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.AddReservationByCustomer.SendPaymentLink
{
    public class SendPaymentLinkCommand : IRequest<Result>
    {
        public int ReservationID { get; set; }
        public int UserID { get; set; }
    }
}
