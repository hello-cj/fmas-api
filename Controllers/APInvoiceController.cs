using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/ap-invoices")]
    [Authorize]
    public class APInvoicesController : ControllerBase
    {
        private readonly FMASDbContext _context;
        private readonly APInvoiceService _service;

        public APInvoicesController(FMASDbContext context, APInvoiceService service)
        {
            _context = context;
            _service = service;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.APInvoices
                .Include(x => x.Vendor)
                .Include(x => x.Lines)
                .OrderByDescending(x => x.InvoiceDate)
                .ToListAsync();

            return Ok(data);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var orgId = Guid.Parse(User.FindFirst("organization_id")?.Value!);

            var invoice = await _context.APInvoices
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.APInvoiceId == id && x.OrganizationId == orgId);

            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(CreateAPInvoiceDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { apInvoiceId = id });
        }

        // =========================
        // POST (Journal Entry)
        // =========================
        [HttpPost("{id}/post")]
        public async Task<IActionResult> Post(Guid id)
        {
            var result = await _service.PostAsync(id);
            return Ok(result);
        }
    }
}