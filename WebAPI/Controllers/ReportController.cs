using Application.Features.Reports;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController(IMediator mediator) : ControllerBase
    {
        [HttpGet("financial-report")]
        public async Task<IActionResult> GetFinancialReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] List<int>? facilityIds) 
        {
            var query = new GetFinancialReportQuery
            {
                StartDate = startDate,
                EndDate = endDate,
                FacilityIds = facilityIds
            };
            var result = await mediator.Send(query);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }
    }
}
