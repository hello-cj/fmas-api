using FMAS.API.DTOs;
using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/ar-payments")]
    [Authorize]
    public class ARPaymentsController : ControllerBase
    {
        private readonly ARPaymentService _service;

        public ARPaymentsController(ARPaymentService service)
        {
            _service = service;
        }

        // =========================
        // GET ALL PAYMENTS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        // =========================
        // GET PAYMENTS BY INVOICE
        // =========================
        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetByInvoice(Guid invoiceId)
        {
            var data = await _service.GetByInvoiceAsync(invoiceId);
            return Ok(data);
        }

        // =========================
        // CREATE PAYMENT
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(CreateARPaymentDto dto)
        {
            var id = await _service.CreateAsync(dto);

            return Ok(new
            {
                paymentId = id
            });
        }
    }
}