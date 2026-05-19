using FMAS.API.DTOs;
using FMAS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/vendors")]
    [Authorize]
    public class VendorsController : ControllerBase
    {
        private readonly VendorService _service;

        public VendorsController(VendorService service)
        {
            _service = service;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();

            return Ok(data);
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var vendor = await _service.GetByIdAsync(id);

            if (vendor == null)
                return NotFound();

            return Ok(vendor);
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(CreateVendorDto dto)
        {
            var id = await _service.CreateAsync(dto);

            return Ok(new { vendorId = id });
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);

            return Ok();
        }
    }
}