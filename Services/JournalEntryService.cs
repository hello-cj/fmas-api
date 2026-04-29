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

        public JournalEntryService(FMASDbContext context, CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
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
                    CreatedBy = userId
                };

                _context.JournalEntries.Add(entry);
                await _context.SaveChangesAsync();

                var totalDebit = dto.Lines.Sum(x => x.Debit);
                var totalCredit = dto.Lines.Sum(x => x.Credit);

                if (totalDebit != totalCredit)
                    throw new Exception("Debits and Credits must be equal");

                foreach (var line in dto.Lines)
                {
                    _context.JournalEntryLines.Add(new JournalEntryLine
                    {
                        JournalEntryId = entry.JournalEntryId,
                        AccountId = line.AccountId,
                        Debit = line.Debit,
                        Credit = line.Credit
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return entry.JournalEntryId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}