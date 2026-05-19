using FMAS.API.Data;
using FMAS.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class TrialBalanceService
    {
        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUser;

        public TrialBalanceService(FMASDbContext context, CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public List<TrialBalanceDto> GetTrialBalance()
        {
            var orgId = _currentUser.OrganizationId.Value;

            var result = _context.Accounts
                .Where(a => a.OrganizationId == orgId)
                .Select(a => new TrialBalanceDto
                {
                    AccountName = a.Name,

                    TotalDebit = _context.JournalEntryLines
                        .Where(l => l.AccountId == a.AccountId &&
                                    l.JournalEntry.OrganizationId == orgId)
                        .Sum(l => (decimal?)l.Debit) ?? 0,

                    TotalCredit = _context.JournalEntryLines
                        .Where(l => l.AccountId == a.AccountId &&
                                    l.JournalEntry.OrganizationId == orgId)
                        .Sum(l => (decimal?)l.Credit) ?? 0,
                })
                .ToList();

            // compute balance
            foreach (var r in result)
            {
                r.Balance = r.TotalDebit - r.TotalCredit;
            }

            return result;
        }
    }
}