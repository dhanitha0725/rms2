using Application.DTOs.PackageDto;
using Application.Features.ManagePackages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController(IMediator mediator) : ControllerBase
    {
        [HttpPost("{facilityId}/packages")]
        public async Task<IActionResult> AddPackage(
            [FromRoute] int facilityId,
            [FromBody] AddPackageDto packageDto)
        {
            // Provide a default id (0) for new packages.
            var command = new AddPackageCommand(facilityId, packageDto);
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result);
        }
    }
}
