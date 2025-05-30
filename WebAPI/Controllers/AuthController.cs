﻿using Application.DTOs.UserDtos;
using Application.Features.AuthenticateUser.LogAdmin;
using Application.Features.AuthenticateUser.LogCustomer;
using Application.Features.AuthenticateUser.RegisterCustomer;
using Application.Features.ManageUsers.AddUser;
using Application.Features.ManageUsers.GetCustomerDetails;
using Application.Features.ManageUsers.GetUserDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
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

        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto loginDto)
        {
            var command = new AdminLoginCommand { LoginDto = loginDto };
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }
            return Ok(new { Token = result.Value });
        }

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
            return Ok(result);
        }

        [HttpGet("get-user-details")]
        public async Task<IActionResult> GetUserDetails()
        {
            var query = new GetUserDetailsQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("get-customer-details")]
        public async Task<IActionResult> GetCustomerDetails()
        {
            var query = new GetCustomerDetailsQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
