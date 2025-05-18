using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using Application.Features.ManagePayments.ConfirmCashPayment;
using Application.Features.ManagePayments.CreatePayment;
using Application.Features.ManagePayments.GetPaymentStatus;
using Application.Features.ManagePayments.UpdatePaymentStatus;
using Application.Features.ManageReservations.ApproveDocument;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController(
        IMediator mediator,
        IGenericRepository<Payment, Guid> paymentRepository,
        IGenericRepository<Reservation, int> reservationRepository,
        IGenericRepository<ReservationUserDetail, int> reservationUserRepository,
        IPayhereService payhereService,
        ILogger logger)
        : ControllerBase
    {
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CreatePaymentCommand request)
        {
            var response = await mediator.Send(request);
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }
            return BadRequest(response.Error);
        }

        [HttpPost("webhook")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Webhook([FromForm] WebhookNotification notification)
        {
            var request = new UpdatePaymentStatusCommand(
                notification.MerchantId,
                notification.OrderId,
                notification.PaymentId,
                notification.payhere_amount.ToString(),
                notification.PayhereCurrency,
                notification.StatusCode,
                notification.Md5Sig);

            var response = await mediator.Send(request);
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }
            return BadRequest(response.Error);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetPaymentStatus([FromQuery] string orderId)
        {
            var query = new GetPaymentStatusQuery
            {
                OrderId = orderId
            };
            var result = await mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("initiate")]
        public async Task<IActionResult> InitiatePayment(string orderId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(orderId))
                return BadRequest(new { Error = "OrderId is required." });

            var payment = (await paymentRepository.GetAllAsync(cancellationToken))
                .FirstOrDefault(p => p.OrderID == orderId);
            if (payment == null || payment.Status != "Pending")
            {
                return BadRequest(new { Error = "Invalid or expired payment request." });
            }

            var reservation = await reservationRepository.GetByIdAsync(payment.ReservationID, cancellationToken);
            var userDetail = await reservationUserRepository.GetByIdAsync(payment.ReservationUserID, cancellationToken);

            if (reservation == null || userDetail == null)
            {
                return BadRequest(new { Error = "Reservation or user details not found." });
            }

            var paymentRequest = new PaymentRequest(
                payment.OrderID,
                payment.AmountPaid ?? 0m,
                "LKR",
                userDetail.FirstName,
                userDetail.LastName,
                userDetail.Email,
                userDetail.PhoneNumber ?? "0000000000",
                "National Institute of Co-Operative Development",
                "Colombo",
                $"Reservation {reservation.ReservationID}");

            try
            {
                var paymentCheckout = payhereService.PrepareCheckout(paymentRequest);
                return Ok(paymentCheckout);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error initiating payment for orderId {OrderId}", orderId);
                return StatusCode(500, new { Error = "Failed to initiate payment." });
            }
        }

        [HttpPost("approve-document")]
        public async Task<IActionResult> ApproveDocument([FromBody] ApproveDocumentCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result);
        }

        [HttpPost("confirm-cash-payment")]
        public async Task<IActionResult> ConfirmCashPayment([FromBody] ConfirmCashPaymentCommand command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.Error);
        }
    }
}
