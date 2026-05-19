using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class APPaymentService
    {
        private readonly FMASDbContext _context;
        private readonly JournalEntryService _journalService;
        private readonly CurrentUserService _currentUser;

        public APPaymentService(
            FMASDbContext context,
            JournalEntryService journalService,
            CurrentUserService currentUser)
        {
            _context = context;
            _journalService = journalService;
            _currentUser = currentUser;
        }

        public async Task<Guid> CreateAsync(CreateAPPaymentDto dto)
        {
            var orgId = _currentUser.OrganizationId!.Value;
            var userId = _currentUser.UserId!.Value;

            var invoice = await _context.APInvoices
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.APInvoiceId == dto.APInvoiceId);

            if (invoice == null)
                throw new Exception("Invoice not found");

            var totalInvoice = invoice.Lines.Sum(x => x.Quantity * x.UnitPrice);

            var paidSoFar = await _context.APPayments
                .Where(x => x.APInvoiceId == dto.APInvoiceId)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            var remaining = totalInvoice - paidSoFar;

            if (dto.Amount > remaining)
                throw new Exception("Payment exceeds remaining balance");

            var payment = new APPayment
            {
                APPaymentId = Guid.NewGuid(),
                APInvoiceId = dto.APInvoiceId,
                OrganizationId = orgId,
                Amount = dto.Amount,
                PaymentDate = DateTime.SpecifyKind(dto.PaymentDate, DateTimeKind.Utc),
                Reference = dto.Reference
            };

            _context.APPayments.Add(payment);

            // =========================
            // JOURNAL ENTRY (CRITICAL)
            // =========================

            var cashAccountId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var apAccountId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            await _journalService.CreateManualAsync(
                orgId,
                userId,
                dto.Reference,
                $"AP Payment - {invoice.Description}",
                new List<(Guid accountId, decimal debit, decimal credit)>
                {
                    // Debit AP liability (reduce liability)
                    (apAccountId, dto.Amount, 0),

                    // Credit Cash (money out)
                    (cashAccountId, 0, dto.Amount)
                }
            );

            await _context.SaveChangesAsync();

            return payment.APPaymentId;
        }
    }
}