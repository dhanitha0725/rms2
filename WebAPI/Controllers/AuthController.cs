using Application.DTOs;
using Application.Features.AddUser;
using Application.Features.LogCustomer;
using Application.Features.RegisterCustomer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Register a new customer
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerDto registerCustomerDto )
        {
            var command = new RegisterCustomerCommand { RegisterCustomerDto = registerCustomerDto };
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(new {Token = result.Value});
        }

        /// <summary>
        /// Log in a customer
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var command = new LoginCommand { LoginDto = loginDto };
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }
            return Ok(new { Token = result.Value });
        }

        /// <summary>
        /// Add User
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] AddUserDto addUserDto)
        {
            var command = new AddUserCommand { AddUserDto = addUserDto };
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            //return Ok(new { Token = result.Value });
            return Ok(result);
        }
    }
}
