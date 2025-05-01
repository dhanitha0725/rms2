using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using Application.Features.ManagePayments.CreatePayment;
using Application.Features.ManagePayments.GetPaymentStatus;
using Application.Features.ManagePayments.UpdatePaymentStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController(
        IMediator mediator)
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
                notification.PayhereAmount,
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
    }
}
