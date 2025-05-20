using Application.Features.ManagePayments.GetInvoice;
using Application.Features.ManagePayments.GetInvoiceTableData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController(IMediator mediator) : ControllerBase
    {
        [HttpGet("invoice-table-data")]
        public async Task<IActionResult> GetInvoiceTableData(CancellationToken cancellationToken)
        {
            var query = new GetInvoiceTableDataQuery();
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceDetails(int invoiceId, CancellationToken cancellationToken)
        {
            var query = new GetInvoiceQuery
            {
                InvoiceId = invoiceId
            };
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }
    }
}
