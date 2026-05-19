using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class ARInvoiceService
    {
        private readonly FMASDbContext _context;
        private readonly JournalEntryService _journalService;
        private readonly CurrentUserService _currentUser;

        public ARInvoiceService(
            FMASDbContext context,
            JournalEntryService journalService,
            CurrentUserService currentUser)
        {
            _context = context;
            _journalService = journalService;
            _currentUser = currentUser;
        }

        // =========================
        // CREATE AR INVOICE
        // =========================
        public async Task<Guid> CreateAsync(CreateARInvoiceDto dto)
        {
            var orgId = _currentUser.OrganizationId
                ?? throw new Exception("Missing OrganizationId");

            var invoice = new ARInvoice
            {
                ARInvoiceId = Guid.NewGuid(),
                OrganizationId = orgId,
                CustomerId = dto.CustomerId,

                InvoiceNumber = string.IsNullOrWhiteSpace(dto.InvoiceNumber)
                    ? GenerateInvoiceNumber()
                    : dto.InvoiceNumber,

                Description = dto.Description ?? "",

                // ✅ FIX: FORCE UTC
                InvoiceDate = DateTime.SpecifyKind(dto.InvoiceDate, DateTimeKind.Utc),
                DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc),

                Status = InvoiceStatus.Draft,
                Lines = new List<ARInvoiceLine>()
            };

            foreach (var line in dto.Lines)
            {
                invoice.Lines.Add(new ARInvoiceLine
                {
                    ARInvoiceLineId = Guid.NewGuid(),
                    Description = line.Description,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice
                });
            }

            _context.ARInvoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice.ARInvoiceId;
        }

        // =========================
        // POST AR INVOICE → JOURNAL ENTRY
        // =========================
        public async Task<string> PostAsync(Guid id)
        {
            var invoice = await _context.ARInvoices
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.ARInvoiceId == id);

            if (invoice == null)
                throw new Exception("Invoice not found");

            if (invoice.Status != InvoiceStatus.Draft)
                throw new Exception("Only draft invoices can be posted");

            var total = invoice.Lines.Sum(x => x.Quantity * x.UnitPrice);

            var arAccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var revenueAccountId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var orgId = _currentUser.OrganizationId!.Value;
            var userId = _currentUser.UserId;

            var journalId = await _journalService.CreateManualAsync(
                orgId,
                userId.Value,
                invoice.InvoiceNumber,
                "AR Invoice Posting",
                new List<(Guid accountId, decimal debit, decimal credit)>
                {
                    (arAccountId, total, 0),
                    (revenueAccountId, 0, total)
                }
            );

            invoice.Status = InvoiceStatus.Posted;
            await _context.SaveChangesAsync();

            return journalId.ToString();
        }

        // =========================
        // HELPER
        // =========================
        private string GenerateInvoiceNumber()
        {
            return $"AR-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..4]}";
        }
    }
}