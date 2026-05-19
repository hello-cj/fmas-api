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
    [Route("api/journal-entries")]
    [Authorize]
    public class JournalEntriesController : Controller
    {
        private readonly JournalEntryService _service;
        private readonly FMASDbContext _context;

        public JournalEntriesController(
            JournalEntryService service,
            FMASDbContext context)
        {
            _service = service;
            _context = context;
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(CreateJournalEntryDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { journalEntryId = id });
        }

        // =========================
        // DebugAll (fu**ing-bug-fix)
        // =========================
        [HttpGet("debug/all")]
        public async Task<IActionResult> DebugAll()
        {
            var count = await _context.JournalEntries.CountAsync();
            return Ok(new { count });
        }

        // =========================
        // GET ALL (LIST PAGE)
        // =========================

        /*
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.JournalEntries.Include(j => j.Lines).ToListAsync());
        }*/

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var organizationId = User.FindFirst("organization_id")?.Value;

            if (organizationId == null)
                return Unauthorized();

            var entries = await _context.JournalEntries
                .Where(j => j.OrganizationId == Guid.Parse(organizationId))
                .Select(j => new
                {
                    id = j.JournalEntryId,
                    date = j.Date,
                    reference = j.Reference,
                    description = j.Description,
                    status = j.Status.ToString(),

                    totalDebit = j.Lines.Sum(l => l.Debit),
                    totalCredit = j.Lines.Sum(l => l.Credit)
                })
                .OrderByDescending(j => j.date)
                .ToListAsync();

            return Ok(entries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var organizationId = User.FindFirst("organization_id")?.Value;

            if (organizationId == null)
                return Unauthorized();

            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .ThenInclude(l => l.Account)
                .FirstOrDefaultAsync(j =>
                    j.JournalEntryId == id &&
                    j.OrganizationId == Guid.Parse(organizationId)
                );

            if (entry == null)
                return NotFound();

            var result = new
            {
                id = entry.JournalEntryId,
                date = entry.Date,
                reference = entry.Reference,
                description = entry.Description,
                status = entry.Status.ToString(),

                lines = entry.Lines.Select(l => new
                {
                    accountId = l.AccountId,
                    accountName = l.Account.Name, //some kind of fix that doesn't work
                    debit = l.Debit,
                    credit = l.Credit
                }),

                totalDebit = entry.Lines.Sum(x => x.Debit),
                totalCredit = entry.Lines.Sum(x => x.Credit)
            };

            return Ok(result);
        }

        [HttpPost("{id}/post")]
        public async Task<IActionResult> Post(Guid id)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .FirstOrDefaultAsync(j => j.JournalEntryId == id);

            if (entry == null)
                return NotFound();

            if (entry.Status != JournalEntryStatus.Draft)
                return BadRequest("Only draft entries can be posted.");

            // 🔥 BUSINESS RULE: must be balanced
            var totalDebit = entry.Lines.Sum(l => l.Debit);
            var totalCredit = entry.Lines.Sum(l => l.Credit);

            if (totalDebit != totalCredit)
                return BadRequest("Entry is not balanced.");

            entry.Status = JournalEntryStatus.Posted;

            await _context.SaveChangesAsync();

            return Ok("Journal Entry Posted");
        }

        [HttpPost("{id}/lock")]
        public async Task<IActionResult> Lock(Guid id)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.JournalEntryId == id);

            if (entry == null)
                return NotFound();

            if (entry.Status != JournalEntryStatus.Posted)
                return BadRequest("Only posted entries can be locked.");

            entry.Status = JournalEntryStatus.Locked;

            await _context.SaveChangesAsync();

            return Ok("Journal Entry Locked");
        }

    }
}