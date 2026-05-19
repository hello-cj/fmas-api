using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using FMAS.API.Services;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class JournalEntryService
    {
        private readonly FMASDbContext _context;
        private readonly CurrentUserService _currentUser;
        private readonly AuditService _auditService;

        public JournalEntryService(FMASDbContext context, CurrentUserService currentUser, AuditService auditService)
        {
            _context = context;
            _currentUser = currentUser;
            _auditService = auditService;
        }
       
        public async Task<Guid> CreateAsync(CreateJournalEntryDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var orgId = _currentUser.OrganizationId.Value;
                var userId = _currentUser.UserId;

                var entry = new JournalEntry
                {
                    JournalEntryId = Guid.NewGuid(),
                    OrganizationId = orgId,
                    Date = dto.Date,
                    Reference = dto.Reference,
                    Description = dto.Description,
                    Status = JournalEntryStatus.Posted,
                    CreatedBy = userId
                };

                var totalDebit = dto.Lines.Sum(x => x.Debit);
                var totalCredit = dto.Lines.Sum(x => x.Credit);

                if (totalDebit != totalCredit)
                    throw new Exception("Debits and Credits must be equal");

                _context.JournalEntries.Add(entry);
                await _context.SaveChangesAsync();

                foreach (var line in dto.Lines)
                {

                    if (line.Debit > 0 && line.Credit > 0)
                    {
                        throw new Exception(
                            "A journal entry line cannot have both debit and credit values."
                        );
                    }

                    if (line.Debit <= 0 && line.Credit <= 0)
                    {
                        throw new Exception(
                            "A journal entry line must have either a debit or credit value."
                        );
                    }

                    _context.JournalEntryLines.Add(new JournalEntryLine
                    {
                        //JournalEntryLineId = Guid.NewGuid(),
                        JournalEntryId = entry.JournalEntryId,
                        AccountId = line.AccountId,
                        Debit = line.Debit,
                        Credit = line.Credit
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _auditService.LogAsync(
                    "CREATE",
                    "JournalEntry",
                    entry.JournalEntryId,
                    $"Created Journal Entry {entry.Reference}"
                 );

                return entry.JournalEntryId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Guid> CreateManualAsync(
            Guid orgId,
            Guid userId,
            string reference,
            string description,
            List<(Guid accountId, decimal debit, decimal credit)> lines)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var entry = new JournalEntry
            {
                JournalEntryId = Guid.NewGuid(),
                OrganizationId = orgId,
                Date = DateTime.UtcNow,
                Reference = reference,
                Description = description,
                CreatedBy = userId
            };

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();

            foreach (var line in lines)
            {
                _context.JournalEntryLines.Add(new JournalEntryLine
                {
                    JournalEntryId = entry.JournalEntryId,
                    AccountId = line.accountId,
                    Debit = line.debit,
                    Credit = line.credit
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return entry.JournalEntryId;
        }

    }
}