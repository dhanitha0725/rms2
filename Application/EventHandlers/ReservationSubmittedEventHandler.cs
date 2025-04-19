using Application.Abstractions.Interfaces;
using Domain.Entities;
using Serilog;

namespace Application.EventHandlers
{
    public class ReservationSubmittedEventHandler(
            IBackgroundTaskQueue queue,
            IEmailService emailService,
            IGenericRepository<ReservationUserDetail, int> reservationUserRepository,
            ILogger logger)
    {
        public void Handle(int reservationId)
        {
            queue.QueueBackgroundWorkItem(async cancellationToken =>
            {
                // Fetch the ReservationUserDetail using reservationId
                var userDetail = await reservationUserRepository
                    .GetAllAsync(cancellationToken)
                    .ContinueWith(task => task.Result.FirstOrDefault(r => r.ReservationID == reservationId), cancellationToken);

                if (userDetail == null || string.IsNullOrEmpty(userDetail.Email))
                {
                    logger.Information($"No email found for reservation ID: {reservationId}.");
                    return;
                }

                var emailContent = $"Reservation {reservationId} has been submitted.";
                await emailService.SendEmailAsync(
                    userDetail.Email, 
                    "Reservation Submitted",
                    emailContent);
            });
        }
    }
}
