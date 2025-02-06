using Application.DTOs;
using Application.Features.RegisterCustomer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Register a new customer
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerDto registerCustomerDto )
        {
            var command = new RegisterCustomerCommand { RegisterCustomerDto = registerCustomerDto };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(new {Token = result.Value});

        }
    }
}
