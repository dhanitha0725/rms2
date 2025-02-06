using Application.DTOs;
using Application.Features.LogCustomer;
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

        /// <summary>
        /// Log in a customer
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var command = new LoginCommand { LoginDto = loginDto };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }
            return Ok(new { Token = result.Value });
        }
    }
}
