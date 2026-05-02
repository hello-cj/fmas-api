using FMAS.API.Data;
using FMAS.API.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class DashboardService
    {

        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUserService;

        public DashboardService(
            FMASDbContext context,
            CurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var orgId = _currentUserService.OrganizationId;

            if (orgId == null)
                throw new Exception("Organization not found in token.");

            var totalEntries = await _context.JournalEntries
                .Where(x => x.OrganizationId == orgId)
                .CountAsync();

            var totalDebit = await (
                from line in _context.JournalEntryLines
                join entry in _context.JournalEntries
                    on line.JournalEntryId equals entry.JournalEntryId
                 where entry.OrganizationId == orgId
                 select line.Debit
            ).SumAsync(x => (decimal?)x) ?? 0;

            var totalCredit = await (
                from line in _context.JournalEntryLines
                join entry in _context.JournalEntries
                    on line.JournalEntryId equals entry.JournalEntryId
                where entry.OrganizationId == orgId
                select line.Credit
            ).SumAsync(x => (decimal?)x) ?? 0;

            var totalUsers = await _context.Users
                .Where(x => x.OrganizationId == orgId)
                .CountAsync();

            return new DashboardSummaryDto
            {
                TotalJournalEntries = totalEntries,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                TotalUsers = totalUsers
            };
        }

        public async Task<List<RecentJournalEntryDto>> GetRecentJournalEntriesAsync(int take = 5)
        {
            var orgId = _currentUserService.OrganizationId;

            if (orgId == null)
                throw new Exception("Organization not found in token.");

            var entries = await _context.JournalEntries
                .Where(e => e.OrganizationId == orgId)
                .OrderByDescending(e => e.Date)
                .Take(take)
                .Select(e => new RecentJournalEntryDto
                {
                    JournalEntryId = e.JournalEntryId,
                    Date = e.Date,
                    Reference = e.Reference,
                    Description = e.Description,

                    TotalDebit = _context.JournalEntryLines
                        .Where(l => l.JournalEntryId == e.JournalEntryId)
                        .Sum(l => (decimal?)l.Debit) ?? 0,

                    TotalCredit = _context.JournalEntryLines
                        .Where(l => l.JournalEntryId == e.JournalEntryId)
                        .Sum(l => (decimal?)l.Credit) ?? 0
                })
                .ToListAsync();

            return entries;
        }

    }
}
