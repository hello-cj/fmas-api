using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class APInvoiceService
    {
        private readonly FMASDbContext _context;
        private readonly JournalEntryService _journalService;
        private readonly CurrentUserService _currentUser;

        public APInvoiceService(
            FMASDbContext context,
            JournalEntryService journalService,
            CurrentUserService currentUser)
        {
            _context = context;
            _journalService = journalService;
            _currentUser = currentUser;
        }

        // =========================
        // CREATE AP INVOICE
        // =========================
        public async Task<Guid> CreateAsync(CreateAPInvoiceDto dto)
        {
            var orgId = _currentUser.OrganizationId
                ?? throw new Exception("Missing OrganizationId");

            var invoice = new APInvoice
            {
                APInvoiceId = Guid.NewGuid(),
                OrganizationId = orgId,
                VendorId = dto.VendorId,
                Reference = dto.Reference,
                Description = dto.Description,
                InvoiceDate = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                Status = InvoiceStatus.Draft
            };

            _context.APInvoices.Add(invoice);
            await _context.SaveChangesAsync();

            foreach (var line in dto.Lines)
            {
                _context.APInvoiceLines.Add(new APInvoiceLine
                {
                    APInvoiceId = invoice.APInvoiceId,
                    AccountId = line.AccountId,
                    Description = line.Description,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    //Total = line.Quantity * line.UnitPrice
                });
            }

            await _context.SaveChangesAsync();

            return invoice.APInvoiceId;
        }

        // =========================
        // POST AP INVOICE → JOURNAL ENTRY
        // =========================
        public async Task<string> PostAsync(Guid id)
        {
            var invoice = await _context.APInvoices
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.APInvoiceId == id);

            if (invoice == null)
                throw new Exception("Invoice not found");

            if (invoice.Status != InvoiceStatus.Draft)
                throw new Exception("Only draft invoices can be posted");

            var total = invoice.Lines.Sum(x => x.Quantity * x.UnitPrice);

            // You will replace these with real chart-of-accounts lookup later
            var expenseAccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var apAccountId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var orgId = _currentUser.OrganizationId!.Value;
            var userId = _currentUser.UserId;

            // CREATE JOURNAL ENTRY
            var journalId = await _journalService.CreateManualAsync(
                orgId,
                userId.Value,
                invoice.Reference,
                $"AP Invoice - {invoice.Description}",
                new List<(Guid accountId, decimal debit, decimal credit)>
                {
                    (expenseAccountId, total, 0),
                    (apAccountId, 0, total)
                }
            );

            invoice.Status = InvoiceStatus.Posted;
            await _context.SaveChangesAsync();

            return journalId.ToString();
        }
    }
}