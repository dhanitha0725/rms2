using Application.Abstractions.Interfaces;
using Application.DTOs.Payment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController(
        IPayhereService payhereService,
        IMediator mediator)
        : ControllerBase
    {
        [HttpPost("checkout")]
        public IActionResult InitiatePayment([FromBody] PaymentRequest paymentRequest)
        {
            var response = payhereService.PrepareCheckout(paymentRequest);
            return Ok(response);
        }

        [HttpPost("webhook")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Webhook([FromBody] WebhookNotification notification)
        {
            var isValid = payhereService.VerifyWebhook(notification);
            if (!isValid)
            {
                return BadRequest("Invalid webhook notification");
            }
            return Ok();
        }
    }
}
