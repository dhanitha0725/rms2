using Application.Features.ManageUsers.GetCustomerDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomerDetails()
        {
            var query = new GetCustomerDetailsQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.Error.Message });
            }
            return Ok(result.Value);
        }
    }
}
