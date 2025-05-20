using Application.DTOs.PackageDto;
using Application.Features.ManagePackages.AddPackage;
using Application.Features.ManagePackages.DeletePackages;
using Application.Features.ManagePackages.GetPackageDetails;
using Application.Features.ManagePackages.UpdatePackages;
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
            var command = new AddPackageCommand(facilityId, packageDto);
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result);
        }

        [HttpGet("packages-details")]
        public async Task<IActionResult> GetPackageDetails()
        {
            var query = new GetPackageDetailsQuery();
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result);
        }

        [HttpPut("update-package/{packageId}")]
        public async Task<IActionResult> UpdatePackage(
            [FromRoute] int packageId,
            [FromBody] UpdatePackageDto packageDto)
        {
            var command = new UpdatePackageCommand { PackageId = packageId, PackageDto = packageDto };
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result);
        }

        [HttpDelete("delete-package/{packageId}")]
        public async Task<IActionResult> DeletePackage(int packageId)
        {
            var command = new DeletePackageCommand { PackageId = packageId };
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result);
        }
    }
}
