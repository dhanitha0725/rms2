using Application.Abstractions.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class EmailContentService(
        IGenericRepository<Reservation, int> reservationRepository)
        : IEmailContentService
    {
        private static string GetCommonStyles()
        {
            return """
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
                   @media screen and (max-width: 600px) {
                       .detail-item {
                           flex-direction: column;
                       }
                       .detail-label {
                           margin-bottom: 4px;
                       }
                   }
                   """;
        }

        private static string GetHeader(string title)
        {
            return $"""
                    <div class="header">
                        <div class="logo">VENUE BOOKINGS</div>
                        <h1 class="title-text">{title}</h1>
                    </div>
                    """;
        }

        private static string GetFooter()
        {
            return """
                   <div class="footer">
                       <div>National Institute of Co-operative Development</div>
                       <div>Address: 123 Co-op Road, Polgolla, Sri Lanka</div>
                       <div>Email: support@nicd.lk | Phone: +94 11 123 4567</div>
                   </div>
                   """;
        }

        private static string GetReservationDetailsSection(string startDate, string endDate)
        {
            return $"""
                    <h2 class="section-title">Reservation Details</h2>
                    
                    <div class="detail-item">
                        <div class="detail-label">Start Date:</div>
                        <div class="detail-value">{startDate}</div>
                    </div>
                    
                    <div class="detail-item">
                        <div class="detail-label">End Date:</div>
                        <div class="detail-value">{endDate}</div>
                    </div>
                    """;
        }

        private static string GetReservationDetailsSectionWithStatus(string startDate, string endDate, string status)
        {
            return $"""
                    <h2 class="section-title">Reservation Details</h2>
                    
                    <div class="detail-item">
                        <div class="detail-label">Status:</div>
                        <div class="detail-value">{status}</div>
                    </div>
                    
                    <div class="detail-item">
                        <div class="detail-label">Start Date:</div>
                        <div class="detail-value">{startDate}</div>
                    </div>
                    
                    <div class="detail-item">
                        <div class="detail-label">End Date:</div>
                        <div class="detail-value">{endDate}</div>
                    </div>
                    """;
        }

        private static string GetEmailWrapper(string title, string body)
        {
            return $"""
                    <!DOCTYPE html>
                    <html lang="en">
                    <head>
                        <meta charset="UTF-8">
                        <meta name="viewport" content="width=device-width, initial-scale=1.0">
                        <title>{title}</title>
                        <style>
                            {GetCommonStyles()}
                        </style>
                    </head>
                    <body>
                        <div class="email-container">
                            {GetHeader(title)}
                            
                            <div class="content">
                                {body}
                            </div>
                            
                            {GetFooter()}
                        </div>
                    </body>
                    </html>
                    """;
        }

        private static string GetReservationIdSection(int reservationId)
        {
            return $"""
                    <div class="reservation-id">
                        Reservation ID: <span>{reservationId}</span>
                    </div>
                    """;
        }

        private static string GetTotalAmountSection(string totalAmount, string label = "Total Amount Due")
        {
            return $"""
                    <div class="total-section">
                        <div>{label}</div>
                        <div class="total-value">{totalAmount}</div>
                    </div>
                    """;
        }

        private static string GetThankYouSection()
        {
            return """
                   <div style="margin-top: 25px; font-style: italic; text-align: center; color: #666;">
                       Thank you for choosing National Institute of Co-operative Development.
                   </div>
                   """;
        }

        private (string startDate, string endDate, string totalAmount) FormatReservationDetails(Reservation reservation)
        {
            string startDate = reservation.StartDate.ToString("yyyy-MM-dd HH:mm");
            string endDate = reservation.EndDate.ToString("yyyy-MM-dd HH:mm");
            string totalAmount = $"Rs. {reservation.Total:N2}";

            return (startDate, endDate, totalAmount);
        }

        public async Task<string> GeneratePendingApprovalEmailBodyAsync(int reservationId, CancellationToken cancellationToken)
        {
            var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation not found for ID: {reservationId}");
            }

            var (startDate, endDate, totalAmount) = FormatReservationDetails(reservation);

            var content = $"""
                           {GetReservationIdSection(reservation.ReservationID)}
                           
                           <p>Dear Guest,</p>
                           
                           <div class="message-box" style="background-color: #f0f7ff; border-left: 4px solid #207cca; padding: 15px; margin: 20px 0; border-radius: 0 5px 5px 0;">
                               <p><strong>Your reservation is currently under review.</strong></p>
                               <p>We will confirm document approval within 48 hours. Once approved, we'll send you an email with payment instructions.</p>
                               <p>You can opt for online payment via the payment link or make a cash payment within two days after your reservation is confirmed.</p>
                           </div>
                           
                           <h2 class="section-title">Reservation Details</h2>
                           
                           <div class="detail-item">
                               <div class="detail-label">Reservation ID:</div>
                               <div class="detail-value">{reservation.ReservationID}</div>
                           </div>
                           
                           <div class="detail-item">
                               <div class="detail-label">Start Date:</div>
                               <div class="detail-value">{startDate}</div>
                           </div>
                           
                           <div class="detail-item">
                               <div class="detail-label">End Date:</div>
                               <div class="detail-value">{endDate}</div>
                           </div>
                           
                           <div class="detail-item">
                               <div class="detail-label">Total Amount:</div>
                               <div class="detail-value">{totalAmount}</div>
                           </div>
                           
                           <div class="important-note">
                               <p><strong>Important:</strong> If your reservation is approved and payment is not received within the specified timeframe, your booking will be automatically canceled.</p>
                           </div>
                           
                           <p>If you have any questions, please don't hesitate to contact our support team.</p>
                           
                           <p>Thank you for choosing our service!</p>
                           """;

            return GetEmailWrapper("Reservation Under Review", content);
        }

        public async Task<string> GeneratePaymentConfirmationEmailBodyAsync(int reservationId, CancellationToken cancellationToken)
        {
            var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation not found for ID: {reservationId}");
            }

            var (startDate, endDate, totalAmount) = FormatReservationDetails(reservation);

            var content = $"""
                           <div class="success-message" style="background-color: #ebf7ed; border-left: 4px solid #28a745; padding: 15px; margin: 20px 0; border-radius: 0 5px 5px 0;">
                               <p><strong>Your payment has been successfully processed.</strong></p>
                               <p>Your reservation is now confirmed. Thank you for your payment.</p>
                           </div>
                           
                           {GetReservationIdSection(reservation.ReservationID)}
                           
                           {GetReservationDetailsSectionWithStatus(startDate, endDate, "Confirmed")}
                           
                           {GetTotalAmountSection(totalAmount, "Total Amount Paid")}
                           
                           <div class="thank-you" style="margin-top: 25px; font-style: italic; text-align: center; color: #666;">
                               Thank you for choosing National Institute of Co-operative Development.
                               We look forward to hosting you!
                           </div>
                           """;

            return GetEmailWrapper("Payment Successful!", content);
        }

        public async Task<string> GenerateBankTransferInstructionsEmailBodyAsync(int reservationId, CancellationToken cancellationToken)
        {
            var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation not found for ID: {reservationId}");
            }

            var (startDate, endDate, totalAmount) = FormatReservationDetails(reservation);

            var content = $"""
                           {GetReservationIdSection(reservation.ReservationID)}
                           
                           <p>Dear Guest,</p>
                           
                           <p>Thank you for uploading your bank transfer receipt for your reservation at the National Institute of Co-operative Development.</p>
                           
                           <div class="payment-status" style="background-color: #f0f7ff; border-left: 4px solid #207cca; padding: 15px; margin: 20px 0; border-radius: 0 5px 5px 0;">
                               <h2 class="section-title">Payment Status</h2>
                               <p><strong>Your payment proof has been received and is currently under review.</strong></p>
                               <p>We will verify your payment and confirm your reservation within <span class="deadline" style="color: #207cca; font-weight: bold;">48 hours</span>. You will receive a confirmation email once your payment is verified.</p>
                           </div>
                           
                           <div class="important-note">
                               <p><strong>Please Note:</strong> If there are any issues with your payment verification, we will contact you via email or phone.</p>
                           </div>
                           
                           {GetReservationDetailsSection(startDate, endDate)}
                           
                           {GetTotalAmountSection(totalAmount, "Amount Paid")}
                           
                           <p>If you have any questions about your payment status, please don't hesitate to contact our support team.</p>
                           
                           <p>Thank you for choosing National Institute of Co-operative Development.</p>
                           """;

            return GetEmailWrapper("Payment Under Review", content);
        }

        public async Task<string> GenerateCashPaymentInstructionsEmailBodyAsync(int reservationId, CancellationToken cancellationToken)
        {
            var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation not found for ID: {reservationId}");
            }

            var (startDate, endDate, totalAmount) = FormatReservationDetails(reservation);
            string paymentDeadline = DateTime.Now.AddDays(2).ToString("dddd, MMMM d, yyyy");

            var content = $"""
                           {GetReservationIdSection(reservation.ReservationID)}
                           
                           <p>Dear Guest,</p>
                           
                           <p>Thank you for choosing the National Institute of Co-operative Development. Your reservation requires payment within <span class="deadline">2 days</span> to be confirmed.</p>
                           
                           <div class="payment-instructions" style="background-color: #f0f7ff; border-left: 4px solid #207cca; padding: 15px; margin: 20px 0; border-radius: 0 5px 5px 0;">
                               <h2 class="section-title">Cash Payment Instructions</h2>
                               <p>Please visit our administrative office to make your cash payment.</p>
                               <p>Be sure to bring your reservation ID and a valid ID document.</p>
                           </div>
                           
                           <div class="office-hours" style="background-color: #e8f5e9; border-left: 4px solid #4caf50; padding: 15px; margin: 20px 0; border-radius: 0 5px 5px 0;">
                               <h3>Office Hours:</h3>
                               <p>Monday to Friday: 9:00 AM - 4:00 PM</p>
                               <p>Saturday: 9:00 AM - 12:00 PM</p>
                               <p>Sunday & Public Holidays: Closed</p>
                           </div>
                           
                           <div class="important-note">
                               <p><strong>Important:</strong> Your payment deadline is <span class="deadline">{paymentDeadline}</span>. If payment is not received by this date, your reservation will be automatically canceled.</p>
                           </div>
                           
                           {GetReservationDetailsSection(startDate, endDate)}
                           
                           {GetTotalAmountSection(totalAmount, "Amount Due")}
                           
                           <p>If you have any questions about the payment process, please don't hesitate to contact our support team.</p>
                           """;

            return GetEmailWrapper("Cash Payment Instructions", content);
        }

        public async Task<string> GeneratePaymentEmailAsync(
            int reservationId,
            string email,
            string paymentLink,
            CancellationToken cancellationToken)
        {
            var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation not found for ID: {reservationId}");
            }

            var (startDate, endDate, totalAmount) = FormatReservationDetails(reservation);

            var content = $"""
                           {GetReservationIdSection(reservation.ReservationID)}
                           
                           <p>Dear Guest,</p>
                           
                           <p>Your reservation has been approved! Please complete the payment to confirm your booking.</p>
                           
                           <div style="text-align: center; margin: 25px 0;">
                               <a href="{paymentLink}" style="display: inline-block; padding: 12px 24px; background-color: #207cca; color: white; text-decoration: none; border-radius: 4px; font-size: 16px;">Pay Now</a>
                           </div>
                           
                           {GetReservationDetailsSectionWithStatus(startDate, endDate, "Approved - Pending Payment")}
                           
                           {GetTotalAmountSection(totalAmount)}
                           
                           <div class="important-note">
                               <p><strong>Important:</strong> Please complete your payment within 30 minutes. If payment is not received within this timeframe, your reservation will be automatically canceled.</p>
                           </div>
                           
                           {GetThankYouSection()}
                           """;

            return GetEmailWrapper("Reservation Approved - Complete Payment", content);
        }
    }
}
