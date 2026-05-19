using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/ap-payments")]
    [Authorize]
    public class APPaymentsController : ControllerBase
    {
        private readonly APPaymentService _service;
        private readonly FMASDbContext _context;

        public APPaymentsController(APPaymentService service, FMASDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAPPaymentDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { paymentId = id });
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoice(Guid invoiceId)
        {
            var payments = await _context.APPayments
                .Where(x => x.APInvoiceId == invoiceId)
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();

            return Ok(payments);
        }
    }
}