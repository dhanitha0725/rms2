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

            string totalAmount = $"Rs. {reservation.Total:N2}";

            var emailHtml = $$"""

                                          <!DOCTYPE html>
                                          <html lang="en">
                                          <head>
                                              <meta charset="UTF-8">
                                              <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                              <title>Reservation Under Review</title>
                                              <style>
                                                  body {
                                                      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                                      line-height: 1.6;
                                                      color: #333;
                                                      margin: 0;
                                                      padding: 0;
                                                      background-color: #f4f4f4;
                                                  }
                                                  .container {
                                                      max-width: 600px;
                                                      margin: 20px auto;
                                                      background: #fff;
                                                      border-radius: 10px;
                                                      box-shadow: 0 0 10px rgba(0,0,0,0.1);
                                                      overflow: hidden;
                                                  }
                                                  .header {
                                                      background: linear-gradient(135deg, #1e5799 0%, #207cca 100%);
                                                      color: white;
                                                      padding: 20px;
                                                      text-align: center;
                                                  }
                                                  .content {
                                                      padding: 20px 30px;
                                                  }
                                                  h1 {
                                                      color: white;
                                                      margin: 0;
                                                      font-size: 24px;
                                                  }
                                                  h2 {
                                                      color: #207cca;
                                                      border-bottom: 1px solid #eee;
                                                      padding-bottom: 10px;
                                                      margin-top: 30px;
                                                  }
                                                  .message-box {
                                                      background-color: #f0f7ff;
                                                      border-left: 4px solid #207cca;
                                                      padding: 15px;
                                                      margin: 20px 0;
                                                      border-radius: 0 5px 5px 0;
                                                  }
                                                  .detail-item {
                                                      margin-bottom: 15px;
                                                      display: flex;
                                                      flex-wrap: wrap;
                                                  }
                                                  .label {
                                                      font-weight: bold;
                                                      width: 120px;
                                                      color: #555;
                                                  }
                                                  .value {
                                                      flex: 1;
                                                  }
                                                  .footer {
                                                      background-color: #f4f4f4;
                                                      padding: 15px;
                                                      text-align: center;
                                                      font-size: 14px;
                                                      color: #666;
                                                      border-top: 1px solid #ddd;
                                                  }
                                                  .important-note {
                                                      background-color: #fff8e1;
                                                      border-left: 4px solid #ffca28;
                                                      padding: 15px;
                                                      margin-top: 20px;
                                                      border-radius: 0 5px 5px 0;
                                                  }
                                                  @media (max-width: 600px) {
                                                      .detail-item {
                                                          flex-direction: column;
                                                      }
                                                      .label {
                                                          width: 100%;
                                                          margin-bottom: 5px;
                                                      }
                                                  }
                                              </style>
                                          </head>
                                          <body>
                                              <div class="container">
                                                  <div class="header">
                                                      <h1>Reservation Under Review</h1>
                                                  </div>
                                                  <div class="content">
                                                      <p>Dear Guest,</p>
                                                      
                                                      <div class="message-box">
                                                          <p><strong>Your reservation is currently under review.</strong></p>
                                                          <p>We will confirm document approval within 48 hours. Once approved, we'll send you an email with payment instructions.</p>
                                                          <p>You can opt for online payment via the payment link or make a cash payment within two days after your reservation is confirmed.</p>
                                                      </div>
                                                      
                                                      <h2>Reservation Details</h2>
                                                      
                                                      <div class="detail-item">
                                                          <div class="label">Reservation ID:</div>
                                                          <div class="value">{{reservation.ReservationID}}</div>
                                                      </div>
                                                      
                                                      <div class="detail-item">
                                                          <div class="label">Start Date:</div>
                                                          <div class="value">{{startDate}}</div>
                                                      </div>
                                                      
                                                      <div class="detail-item">
                                                          <div class="label">End Date:</div>
                                                          <div class="value">{{endDate}}</div>
                                                      </div>
                                                      
                                                      <div class="detail-item">
                                                          <div class="label">Total Amount:</div>
                                                          <div class="value">{{totalAmount}}</div>
                                                      </div>
                                                      
                                                      <div class="important-note">
                                                          <p><strong>Important:</strong> If your reservation is approved and payment is not received within the specified timeframe, your booking will be automatically canceled.</p>
                                                      </div>
                                                      
                                                      <p>If you have any questions, please don't hesitate to contact our support team.</p>
                                                      
                                                      <p>Thank you for choosing our service!</p>
                                                  </div>
                                                  <div class="footer">
                                                      National Institute of Co-operative Development<br>
                                                      Email: support@nicd.lk | Phone: +94 11 123 4567
                                                  </div>
                                              </div>
                                          </body>
                                          </html>
                              """;

            return emailHtml;
        }

        public async Task<string> GeneratePaymentConfirmationEmailBodyAsync(int reservationId, CancellationToken cancellationToken)
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

            string totalAmount = $"Rs. {reservation.Total:N2}";

            var emailHtml = $$"""

                                          <!DOCTYPE html>
                                          <html lang="en">
                                          <head>
                                              <meta charset="UTF-8">
                                              <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                              <title>Payment Confirmation</title>
                                              <style>
                                                  body {
                                                      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                                      line-height: 1.6;
                                                      color: #333;
                                                      margin: 0;
                                                      padding: 0;
                                                      background-color: #f9f9f9;
                                                  }
                                                  .email-container {
                                                      max-width: 600px;
                                                      margin: 0 auto;
                                                      background-color: #ffffff;
                                                      border-radius: 8px;
                                                      overflow: hidden;
                                                      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
                                                  }
                                                  .header {
                                                      background: linear-gradient(135deg, #1e5799 0%, #207cca 100%);
                                                      color: white;
                                                      padding: 30px 20px;
                                                      text-align: center;
                                                  }
                                                  .logo {
                                                      margin-bottom: 15px;
                                                      font-size: 24px;
                                                      font-weight: bold;
                                                  }
                                                  .confirmation-text {
                                                      font-size: 22px;
                                                      margin: 0;
                                                  }
                                                  .content {
                                                      padding: 30px 25px;
                                                  }
                                                  .reservation-id {
                                                      background-color: #f5f9ff;
                                                      border-left: 4px solid #207cca;
                                                      padding: 10px 15px;
                                                      margin-bottom: 25px;
                                                      font-size: 16px;
                                                  }
                                                  .reservation-id span {
                                                      font-weight: bold;
                                                      color: #207cca;
                                                  }
                                                  .reservation-details {
                                                      margin-bottom: 25px;
                                                  }
                                                  .section-title {
                                                      color: #207cca;
                                                      border-bottom: 1px solid #eaeaea;
                                                      padding-bottom: 8px;
                                                      margin-top: 25px;
                                                      margin-bottom: 15px;
                                                      font-size: 18px;
                                                  }
                                                  .detail-item {
                                                      margin-bottom: 15px;
                                                      display: flex;
                                                      flex-wrap: wrap;
                                                  }
                                                  .detail-label {
                                                      flex: 0 0 130px;
                                                      font-weight: bold;
                                                      color: #555;
                                                  }
                                                  .detail-value {
                                                      flex: 1;
                                                      color: #333;
                                                  }
                                                  .success-message {
                                                      background-color: #ebf7ed;
                                                      border-left: 4px solid #28a745;
                                                      padding: 15px;
                                                      margin: 20px 0;
                                                      border-radius: 0 5px 5px 0;
                                                  }
                                                  .total-section {
                                                      background-color: #f5f9ff;
                                                      padding: 15px;
                                                      border-radius: 6px;
                                                      text-align: right;
                                                      margin-top: 20px;
                                                  }
                                                  .total-value {
                                                      font-size: 20px;
                                                      font-weight: bold;
                                                      color: #207cca;
                                                  }
                                                  .footer {
                                                      background-color: #f5f5f5;
                                                      padding: 20px;
                                                      text-align: center;
                                                      font-size: 14px;
                                                      color: #777;
                                                  }
                                                  .thank-you {
                                                      margin-top: 25px;
                                                      font-style: italic;
                                                      text-align: center;
                                                      color: #666;
                                                  }
                                                  @media screen and (max-width: 600px) {
                                                      .detail-item {
                                                          flex-direction: column;
                                                      }
                                                      .detail-label {
                                                          margin-bottom: 4px;
                                                      }
                                                  }
                                              </style>
                                          </head>
                                          <body>
                                              <div class="email-container">
                                                  <div class="header">
                                                      <div class="logo">VENUE BOOKINGS</div>
                                                      <h1 class="confirmation-text">Payment Successful!</h1>
                                                  </div>
                                                  
                                                  <div class="content">
                                                      <div class="success-message">
                                                          <p><strong>Your payment has been successfully processed.</strong></p>
                                                          <p>Your reservation is now confirmed. Thank you for your payment.</p>
                                                      </div>
                                                      
                                                      <div class="reservation-id">
                                                          Reservation ID: <span>{{reservation.ReservationID}}</span>
                                                      </div>
                                                      
                                                      <div class="reservation-details">
                                                          <h2 class="section-title">Reservation Details</h2>
                                                          
                                                          <div class="detail-item">
                                                              <div class="detail-label">Status:</div>
                                                              <div class="detail-value">Confirmed</div>
                                                          </div>
                                                          
                                                          <div class="detail-item">
                                                              <div class="detail-label">Start Date:</div>
                                                              <div class="detail-value">{{startDate}}</div>
                                                          </div>
                                                          
                                                          <div class="detail-item">
                                                              <div class="detail-label">End Date:</div>
                                                              <div class="detail-value">{{endDate}}</div>
                                                          </div>
                                                      </div>
                                                      
                                                      <div class="total-section">
                                                          <div>Total Amount Paid</div>
                                                          <div class="total-value">{{totalAmount}}</div>
                                                      </div>
                                                      
                                                      <div class="thank-you">
                                                          Thank you for choosing National Institute of Co-operative Development.
                                                          We look forward to hosting you!
                                                      </div>
                                                  </div>
                                                  
                                                  <div class="footer">
                                                      <div>National Institute of Co-operative Development</div>
                                                      <div class="contact-info">Email: support@nicd.lk | Phone: +94 11 123 4567</div>
                                                  </div>
                                              </div>
                                          </body>
                                          </html>
                              """;

            return emailHtml;
        }

        public async Task<string> GenerateBankTransferInstructionsEmailBodyAsync(int reservationId, CancellationToken cancellationToken)
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

            string totalAmount = $"Rs. {reservation.Total:N2}";

            var emailHtml = $$"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Bank Transfer Payment Received</title>
                    <style>
                        body {
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            margin: 0;
                            padding: 0;
                            background-color: #f9f9f9;
                        }
                        .email-container {
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            border-radius: 8px;
                            overflow: hidden;
                            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
                        }
                        .header {
                            background: linear-gradient(135deg, #1e5799 0%, #207cca 100%);
                            color: white;
                            padding: 30px 20px;
                            text-align: center;
                        }
                        .logo {
                            margin-bottom: 15px;
                            font-size: 24px;
                            font-weight: bold;
                        }
                        .title-text {
                            font-size: 22px;
                            margin: 0;
                        }
                        .content {
                            padding: 30px 25px;
                        }
                        .reservation-id {
                            background-color: #f5f9ff;
                            border-left: 4px solid #207cca;
                            padding: 10px 15px;
                            margin-bottom: 25px;
                            font-size: 16px;
                        }
                        .reservation-id span {
                            font-weight: bold;
                            color: #207cca;
                        }
                        .section-title {
                            color: #207cca;
                            border-bottom: 1px solid #eaeaea;
                            padding-bottom: 8px;
                            margin-top: 25px;
                            margin-bottom: 15px;
                            font-size: 18px;
                        }
                        .detail-item {
                            margin-bottom: 15px;
                            display: flex;
                            flex-wrap: wrap;
                        }
                        .detail-label {
                            flex: 0 0 130px;
                            font-weight: bold;
                            color: #555;
                        }
                        .detail-value {
                            flex: 1;
                            color: #333;
                        }
                        .payment-status {
                            background-color: #f0f7ff;
                            border-left: 4px solid #207cca;
                            padding: 15px;
                            margin: 20px 0;
                            border-radius: 0 5px 5px 0;
                        }
                        .total-section {
                            background-color: #f5f9ff;
                            padding: 15px;
                            border-radius: 6px;
                            text-align: right;
                            margin-top: 20px;
                        }
                        .total-value {
                            font-size: 20px;
                            font-weight: bold;
                            color: #207cca;
                        }
                        .important-note {
                            background-color: #fff8e1;
                            border-left: 4px solid #ffca28;
                            padding: 15px;
                            margin-top: 20px;
                            border-radius: 0 5px 5px 0;
                        }
                        .footer {
                            background-color: #f5f5f5;
                            padding: 20px;
                            text-align: center;
                            font-size: 14px;
                            color: #777;
                        }
                        .deadline {
                            color: #207cca;
                            font-weight: bold;
                        }
                        @media screen and (max-width: 600px) {
                            .detail-item {
                                flex-direction: column;
                            }
                            .detail-label {
                                margin-bottom: 4px;
                            }
                        }
                    </style>
                </head>
                <body>
                    <div class="email-container">
                        <div class="header">
                            <div class="logo">VENUE BOOKINGS</div>
                            <h1 class="title-text">Payment Under Review</h1>
                        </div>
                        
                        <div class="content">
                            <div class="reservation-id">
                                Reservation ID: <span>{{reservation.ReservationID}}</span>
                            </div>
                            
                            <p>Dear Guest,</p>
                            
                            <p>Thank you for uploading your bank transfer receipt for your reservation at the National Institute of Co-operative Development.</p>
                            
                            <div class="payment-status">
                                <h2 class="section-title">Payment Status</h2>
                                <p><strong>Your payment proof has been received and is currently under review.</strong></p>
                                <p>We will verify your payment and confirm your reservation within <span class="deadline">48 hours</span>. You will receive a confirmation email once your payment is verified.</p>
                            </div>
                            
                            <div class="important-note">
                                <p><strong>Please Note:</strong> If there are any issues with your payment verification, we will contact you via email or phone.</p>
                            </div>
                            
                            <h2 class="section-title">Reservation Details</h2>
                            
                            <div class="detail-item">
                                <div class="detail-label">Start Date:</div>
                                <div class="detail-value">{{startDate}}</div>
                            </div>
                            
                            <div class="detail-item">
                                <div class="detail-label">End Date:</div>
                                <div class="detail-value">{{endDate}}</div>
                            </div>
                            
                            <div class="total-section">
                                <div>Amount Paid</div>
                                <div class="total-value">{{totalAmount}}</div>
                            </div>
                            
                            <p>If you have any questions about your payment status, please don't hesitate to contact our support team.</p>
                            
                            <p>Thank you for choosing National Institute of Co-operative Development.</p>
                        </div>
                        
                        <div class="footer">
                            <div>National Institute of Co-operative Development</div>
                            <div>Email: support@nicd.lk | Phone: +94 11 123 4567</div>
                        </div>
                    </div>
                </body>
                </html>
            """;

            return emailHtml;
        }

        public async Task<string> GenerateCashPaymentInstructionsEmailBodyAsync(int reservationId, CancellationToken cancellationToken)
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

            string totalAmount = $"Rs. {reservation.Total:N2}";
            string paymentDeadline = DateTime.Now.AddDays(2).ToString("dddd, MMMM d, yyyy");

            var emailHtml = $$"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Cash Payment Instructions</title>
                    <style>
                        body {
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            margin: 0;
                            padding: 0;
                            background-color: #f9f9f9;
                        }
                        .email-container {
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            border-radius: 8px;
                            overflow: hidden;
                            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
                        }
                        .header {
                            background: linear-gradient(135deg, #1e5799 0%, #207cca 100%);
                            color: white;
                            padding: 30px 20px;
                            text-align: center;
                        }
                        .logo {
                            margin-bottom: 15px;
                            font-size: 24px;
                            font-weight: bold;
                        }
                        .title-text {
                            font-size: 22px;
                            margin: 0;
                        }
                        .content {
                            padding: 30px 25px;
                        }
                        .reservation-id {
                            background-color: #f5f9ff;
                            border-left: 4px solid #207cca;
                            padding: 10px 15px;
                            margin-bottom: 25px;
                            font-size: 16px;
                        }
                        .reservation-id span {
                            font-weight: bold;
                            color: #207cca;
                        }
                        .section-title {
                            color: #207cca;
                            border-bottom: 1px solid #eaeaea;
                            padding-bottom: 8px;
                            margin-top: 25px;
                            margin-bottom: 15px;
                            font-size: 18px;
                        }
                        .detail-item {
                            margin-bottom: 15px;
                            display: flex;
                            flex-wrap: wrap;
                        }
                        .detail-label {
                            flex: 0 0 130px;
                            font-weight: bold;
                            color: #555;
                        }
                        .detail-value {
                            flex: 1;
                            color: #333;
                        }
                        .payment-instructions {
                            background-color: #f0f7ff;
                            border-left: 4px solid #207cca;
                            padding: 15px;
                            margin: 20px 0;
                            border-radius: 0 5px 5px 0;
                        }
                        .total-section {
                            background-color: #f5f9ff;
                            padding: 15px;
                            border-radius: 6px;
                            text-align: right;
                            margin-top: 20px;
                        }
                        .total-value {
                            font-size: 20px;
                            font-weight: bold;
                            color: #207cca;
                        }
                        .important-note {
                            background-color: #fff8e1;
                            border-left: 4px solid #ffca28;
                            padding: 15px;
                            margin-top: 20px;
                            border-radius: 0 5px 5px 0;
                        }
                        .footer {
                            background-color: #f5f5f5;
                            padding: 20px;
                            text-align: center;
                            font-size: 14px;
                            color: #777;
                        }
                        .deadline {
                            color: #d32f2f;
                            font-weight: bold;
                        }
                        .office-hours {
                            background-color: #e8f5e9;
                            border-left: 4px solid #4caf50;
                            padding: 15px;
                            margin: 20px 0;
                            border-radius: 0 5px 5px 0;
                        }
                        @media screen and (max-width: 600px) {
                            .detail-item {
                                flex-direction: column;
                            }
                            .detail-label {
                                margin-bottom: 4px;
                            }
                        }
                    </style>
                </head>
                <body>
                    <div class="email-container">
                        <div class="header">
                            <div class="logo">VENUE BOOKINGS</div>
                            <h1 class="title-text">Cash Payment Instructions</h1>
                        </div>
                        
                        <div class="content">
                            <div class="reservation-id">
                                Reservation ID: <span>{{reservation.ReservationID}}</span>
                            </div>
                            
                            <p>Dear Guest,</p>
                            
                            <p>Thank you for choosing the National Institute of Co-operative Development. Your reservation requires payment within <span class="deadline">2 days</span> to be confirmed.</p>
                            
                            <div class="payment-instructions">
                                <h2 class="section-title">Cash Payment Instructions</h2>
                                <p>Please visit our administrative office to make your cash payment.</p>
                                <p>Be sure to bring your reservation ID and a valid ID document.</p>
                            </div>
                            
                            <div class="office-hours">
                                <h3>Office Hours:</h3>
                                <p>Monday to Friday: 9:00 AM - 4:00 PM</p>
                                <p>Saturday: 9:00 AM - 12:00 PM</p>
                                <p>Sunday & Public Holidays: Closed</p>
                            </div>
                            
                            <div class="important-note">
                                <p><strong>Important:</strong> Your payment deadline is <span class="deadline">{{paymentDeadline}}</span>. If payment is not received by this date, your reservation will be automatically canceled.</p>
                            </div>
                            
                            <h2 class="section-title">Reservation Details</h2>
                            
                            <div class="detail-item">
                                <div class="detail-label">Start Date:</div>
                                <div class="detail-value">{{startDate}}</div>
                            </div>
                            
                            <div class="detail-item">
                                <div class="detail-label">End Date:</div>
                                <div class="detail-value">{{endDate}}</div>
                            </div>
                            
                            <div class="total-section">
                                <div>Amount Due</div>
                                <div class="total-value">{{totalAmount}}</div>
                            </div>
                            
                            <p>If you have any questions about the payment process, please don't hesitate to contact our support team.</p>
                        </div>
                        
                        <div class="footer">
                            <div>National Institute of Co-operative Development</div>
                            <div>Address: 123 Co-op Road, Polgolla, Sri Lanka</div>
                            <div>Email: support@nicd.lk | Phone: +94 11 123 4567</div>
                        </div>
                    </div>
                </body>
                </html>
            """;

            return emailHtml;
        }
    }
}
