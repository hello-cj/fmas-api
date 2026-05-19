using FMAS.API.Data;
using FMAS.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class FinancialStatementService
    {
        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUser;

        public FinancialStatementService(
            FMASDbContext context,
            CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // ================= INCOME STATEMENT =================
        public List<IncomeStatementDto> GetIncomeStatement()
        {
            var orgId = _currentUser.OrganizationId!.Value;

            var accounts = _context.Accounts
                .Where(a =>
                    a.OrganizationId == orgId &&
                    (a.AccountType == "Revenue" ||
                     a.AccountType == "Expense"))
                .ToList();

            // Preload journal lines once (IMPORTANT OPTIMIZATION)
            var accountIds = accounts.Select(a => a.AccountId).ToList();

            var lines = _context.JournalEntryLines
                .Where(l => accountIds.Contains(l.AccountId))
                .ToList()
                .GroupBy(l => l.AccountId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Debit = g.Sum(x => x.Debit),
                        Credit = g.Sum(x => x.Credit)
                    });

            var result = new List<IncomeStatementDto>();

            foreach (var account in accounts)
            {
                lines.TryGetValue(account.AccountId, out var values);

                var debit = values?.Debit ?? 0;
                var credit = values?.Credit ?? 0;

                decimal amount =
                    account.AccountType == "Revenue"
                        ? credit - debit
                        : debit - credit;

                result.Add(new IncomeStatementDto
                {
                    AccountName = account.Name,
                    Amount = amount,
                    Type = account.AccountType
                });
            }

            return result;
        }

        // ================= BALANCE SHEET =================
        public List<BalanceSheetDto> GetBalanceSheet()
        {
            var orgId = _currentUser.OrganizationId!.Value;

            var accounts = _context.Accounts
                .Where(a =>
                    a.OrganizationId == orgId &&
                    (a.AccountType == "Asset" ||
                     a.AccountType == "Liability" ||
                     a.AccountType == "Equity"))
                .ToList();

            var accountIds = accounts.Select(a => a.AccountId).ToList();

            var lines = _context.JournalEntryLines
                .Where(l => accountIds.Contains(l.AccountId))
                .ToList()
                .GroupBy(l => l.AccountId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Debit = g.Sum(x => x.Debit),
                        Credit = g.Sum(x => x.Credit)
                    });

            var result = new List<BalanceSheetDto>();

            foreach (var account in accounts)
            {
                lines.TryGetValue(account.AccountId, out var values);

                var debit = values?.Debit ?? 0;
                var credit = values?.Credit ?? 0;

                decimal amount =
                    account.AccountType == "Asset"
                        ? debit - credit
                        : credit - debit;

                result.Add(new BalanceSheetDto
                {
                    AccountName = account.Name,
                    Type = account.AccountType,
                    Amount = amount
                });
            }

            return result;
        }
    }
}