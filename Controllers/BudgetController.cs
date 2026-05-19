using FMAS.API.Data;
using FMAS.API.Services;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/budgets")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly FMASDbContext _context;
    private readonly CurrentUserService _currentUser;
    private readonly AuditService _auditService;

    public BudgetController(FMASDbContext context, CurrentUserService currentUser, AuditService auditService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orgId = User.FindFirst("organization_id")?.Value;

        var data = await _context.Budgets
            .Include(b => b.Lines)
            .Where(b => b.OrganizationId.ToString() == orgId)
            .ToListAsync();

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBudgetDto dto)
    {
        var orgId = _currentUser.OrganizationId
            ?? throw new Exception("No organization found");

        if (dto.Lines == null || !dto.Lines.Any())
            return BadRequest("Budget must have at least one line");

        var budget = new Budget
        {
            BudgetId = Guid.NewGuid(),
            OrganizationId = orgId,
            Name = dto.Name,
            StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc),

            Lines = dto.Lines.Select(l => new BudgetLine
            {
                BudgetLineId = Guid.NewGuid(),
                AccountId = l.AccountId,
                Amount = l.Amount
            }).ToList()
        };

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("CREATE", "Budget", budget.BudgetId, $"Created Budget: {budget.Name}");

        return Ok(budget.BudgetId);
    }

    [HttpGet("vs-actual/{id}")]
    public async Task<IActionResult> GetVsActual(Guid id, Guid? accountId)
    {
        var orgId = _currentUser.OrganizationId.Value;



        var budget = await _context.Budgets
            .Include(b => b.Lines)
            .FirstOrDefaultAsync(b =>
                b.BudgetId == id &&
                b.OrganizationId == orgId);

        if (budget == null)
            return NotFound("Budget not found");

        var query = _context.JournalEntryLines
            .Include(l => l.JournalEntry)
            .Include(l => l.Account) //hehe name is showing now
            .Where(l =>
                l.JournalEntry.OrganizationId == orgId &&
                l.JournalEntry.Status != JournalEntryStatus.Draft);

        if (accountId.HasValue)
        {
            query = query.Where(l => l.AccountId == accountId.Value);
        }

        var actuals = await query.ToListAsync();

        var result = budget.Lines.Select(b =>
        {
            var actual = actuals
                .Where(a => a.AccountId == b.AccountId)
                .Sum(a => a.Debit - a.Credit);

            var accountName = _context.Accounts
                .Where(a => a.AccountId == b.AccountId && a.OrganizationId == orgId)
                .Select(a => a.Name)
                .FirstOrDefault();

            return new
            {
                accountId = b.AccountId,
                accountName = accountName,
                budget = b.Amount,
                actual = actual,
                variance = b.Amount - actual
            };
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var orgId = _currentUser.OrganizationId.Value;

        var budget = await _context.Budgets
            .Include(b => b.Lines)
            .FirstOrDefaultAsync(b =>
                b.BudgetId == id &&
                b.OrganizationId == orgId);

        if (budget == null)
            return NotFound("Budget not found");

        return Ok(budget);
    }
}