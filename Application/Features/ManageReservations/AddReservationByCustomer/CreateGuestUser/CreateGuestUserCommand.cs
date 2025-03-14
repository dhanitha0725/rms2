using Domain.Common;
using MediatR;

namespace Application.Features.ManageReservations.AddReservationByCustomer.CreateGuestUser
{
    public class CreateGuestUserCommand : IRequest<Result<int>>
    {
        public string GuestFirstName { get; set; }
        public string GuestLastName { get; set; }
        public string GuestEmail { get; set; }
        public string GuestPhone { get; set; }
    }
}
