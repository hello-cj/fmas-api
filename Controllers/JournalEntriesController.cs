using FMAS.API.DTOs;
using FMAS.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FMAS.API.Controllers
{
    [ApiController]
    [Route("api/journal-entries")]
    [Authorize]
    public class JournalEntriesController : Controller
    {
        private readonly JournalEntryService _service;

        public JournalEntriesController(JournalEntryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateJournalEntryDto dto)
        {
            var orgId = Guid.Parse(User.FindFirst("organization_id").Value);

            var id = await _service.CreateAsync(dto);

            return Ok(new { journalEntryId = id });
        }
    }
}
