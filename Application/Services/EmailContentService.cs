using Application.Abstractions.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class EmailContentService(
        IGenericRepository<Reservation, int> reservationRepository)
        : IEmailContentService
    {
        public async Task<string> GeneratePendingApprovalEmailBodyAsync(int reservationId, CancellationToken cancellationToken)
        {
            // Fetch reservation
            var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation not found for ID: {reservationId}");
            }

            // Format the reservation start date and end date
            string startDate = reservation.StartDate.ToString("yyyy-MM-dd HH:mm");
            string endDate = reservation.EndDate.ToString("yyyy-MM-dd HH:mm");
            string totalAmount = reservation.Total.ToString("C");

            // Build simplified email HTML
            var emailHtml = $$"""

                                          <!DOCTYPE html>
                                          <html lang="en">
                                          <head>
                                              <meta charset="UTF-8">
                                              <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                              <title>Reservation Information</title>
                                              <style>
                                                  body {
                                                      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                                      line-height: 1.6;
                                                      color: #333;
                                                      margin: 0;
                                                      padding: 20px;
                                                  }
                                                  .container {
                                                      max-width: 600px;
                                                      margin: 0 auto;
                                                  }
                                                  h1 {
                                                      color: #207cca;
                                                  }
                                                  .detail-item {
                                                      margin-bottom: 15px;
                                                  }
                                                  .label {
                                                      font-weight: bold;
                                                  }
                                              </style>
                                          </head>
                                          <body>
                                              <div class="container">
                                                  <h1>Reservation Details</h1>
                                                  
                                                  <div class="detail-item">
                                                      <span class="label">Reservation ID:</span> {{reservation.ReservationID}}
                                                  </div>
                                                  
                                                  <div class="detail-item">
                                                      <span class="label">Start Date:</span> {{startDate}}
                                                  </div>
                                                  
                                                  <div class="detail-item">
                                                      <span class="label">End Date:</span> {{endDate}}
                                                  </div>
                                                  
                                                  <div class="detail-item">
                                                      <span class="label">Total Amount:</span> {{totalAmount}}
                                                  </div>
                                              </div>
                                          </body>
                                          </html>
                              """;

            return emailHtml;
        }
    }
}
