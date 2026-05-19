using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class LedgerService
    {
        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUser;

        public LedgerService(
            FMASDbContext context,
            CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<LedgerEntryDto>> GetAccountLedgerAsync(
            Guid accountId,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var orgId = _currentUser.OrganizationId.Value;

            var query = _context.JournalEntryLines
                .AsNoTracking()
                .Include(l => l.JournalEntry)
                .Where(l =>
                    l.AccountId == accountId &&
                    l.JournalEntry.OrganizationId == orgId
                    //l.JournalEntry.Status != JournalEntryStatus.Draft
                );

            // =========================
            // SAFE DATE HANDLING (UTC FIX)
            // =========================
            if (fromDate.HasValue)
            {
                var fromUtc = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);

                query = query.Where(l =>
                    l.JournalEntry.Date >= fromUtc);
            }

            if (toDate.HasValue)
            {
                var toUtc = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);

                query = query.Where(l =>
                    l.JournalEntry.Date <= toUtc);
            }

            var entries = await query
                .OrderBy(l => l.JournalEntry.Date)
                .ThenBy(l => l.JournalEntry.JournalEntryId)
                .Select(l => new
                {
                    Date = l.JournalEntry.Date,
                    Reference = l.JournalEntry.Reference,
                    Description = l.JournalEntry.Description,
                    Debit = l.Debit,
                    Credit = l.Credit
                })
                .ToListAsync();

            decimal runningBalance = 0;

            var ledger = new List<LedgerEntryDto>();

            foreach (var e in entries)
            {
                runningBalance += (e.Debit - e.Credit);

                ledger.Add(new LedgerEntryDto
                {
                    Date = e.Date,
                    Reference = e.Reference,
                    Description = e.Description,
                    Debit = e.Debit,
                    Credit = e.Credit,
                    Balance = runningBalance
                });
            }

            return ledger;
        }
    }
}