using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.ManageReservations.AddReservationByCustomer.SendPaymentLink
{
    public class SendPaymentLinkCommandHandler (
        IEmailService emailService,
        IGenericRepository<User,int> userRepository) 
        : IRequestHandler<SendPaymentLinkCommand, Result>
    {
        public async Task<Result> Handle(
            SendPaymentLinkCommand request, 
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.UserID, cancellationToken);
            if (user == null)
            {
                return Result.Failure(new Error("User not found"));
            }

            var paymentLink = $"https://payment.com?reservationId={request.ReservationID}";
            await emailService.SendEmailAsync(
                user.Email, 
                "Approval of the Reservation",
                $"Please complete your payment here: {paymentLink}");

            return Result.Success([]);
        }
    }
}
