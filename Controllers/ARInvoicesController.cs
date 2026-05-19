using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/ar-invoices")]
    [Authorize]
    public class ARInvoicesController : ControllerBase
    {
        private readonly ARInvoiceService _service;
        private readonly FMASDbContext _context;

        public ARInvoicesController(ARInvoiceService service, FMASDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateARInvoiceDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { arInvoiceId = id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.ARInvoices
                .Include(x => x.Customer)
                .Include(x => x.Lines)
                .OrderByDescending(x => x.InvoiceDate)
                .ToListAsync();

            return Ok(data);
        }

        [HttpPost("{id}/post")]
        public async Task<IActionResult> Post(Guid id)
        {
            var result = await _service.PostAsync(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var invoice = await _context.ARInvoices
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.ARInvoiceId == id);

            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }
    }
}