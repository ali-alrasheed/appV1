using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceRepository repository;

        public InvoicesController(IInvoiceRepository repository)
        {
            this.repository = repository;
        }
        [HttpGet("LastMonth")]
        public async Task<ActionResult> InvoicesInLastMonth()
        {
            var invoices = await repository.GetInLastMonth();
            return Ok(invoices);
        }
        [HttpGet("LastYear")]
        public async Task<ActionResult> InvoicesInLastYear()
        {
            var invoices = await repository.GetInLastYear();
            return Ok(invoices);
        }
        [HttpGet("LastSevenDays")]
        public async Task<ActionResult> InvoicesInLastSevenDays()
        {
            var invoices = await repository.GetInLastSevenDays();
            return Ok(invoices);
        }
    }
}
