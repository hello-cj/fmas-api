using FMAS.API.Data;
using FMAS.API.DTOs;
using FMAS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMAS.API.Services
{
    public class ARPaymentService
    {
        private readonly FMASDbContext _context;
        private readonly JournalEntryService _journalService;
        private readonly CurrentUserService _currentUser;
        private readonly AuditService _auditService;

        public ARPaymentService(
            FMASDbContext context,
            JournalEntryService journalService,
            CurrentUserService currentUser,
            AuditService auditService)
        {
            _context = context;
            _journalService = journalService;
            _currentUser = currentUser;
            _auditService = auditService;
        }

        // =========================
        // CREATE PAYMENT
        // =========================
        public async Task<Guid> CreateAsync(CreateARPaymentDto dto)
        {
            var orgId = _currentUser.OrganizationId.Value;
            var userId = _currentUser.UserId;

            var invoice = await _context.ARInvoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.ARInvoiceId == dto.ARInvoiceId);

            if (invoice == null)
                throw new Exception("Invoice not found");

            var invoiceTotal = invoice.Lines.Sum(l => l.Quantity * l.UnitPrice);

            var paidSoFar = await _context.ARPayments
                .Where(p => p.ARInvoiceId == dto.ARInvoiceId)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var remaining = invoiceTotal - paidSoFar;

            if (dto.Amount > remaining)
                throw new Exception("Payment exceeds remaining balance");

            var payment = new ARPayment
            {
                ARPaymentId = Guid.NewGuid(),
                OrganizationId = orgId,
                CustomerId = dto.CustomerId,
                ARInvoiceId = dto.ARInvoiceId,
                Amount = dto.Amount,
                PaymentDate = DateTime.SpecifyKind(dto.PaymentDate, DateTimeKind.Utc),
                Reference = dto.Reference
            };

            _context.ARPayments.Add(payment);

            invoice.PaidAmount = paidSoFar + dto.Amount;

            invoice.Status =
                invoice.PaidAmount >= invoiceTotal
                    ? InvoiceStatus.Paid
                    : InvoiceStatus.Posted;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "CREATE",
                "ARPayment",
                payment.ARPaymentId,
                $"Created AR Payment {payment.Reference}"
            );

            return payment.ARPaymentId;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<ARPayment>> GetAllAsync()
        {
            var orgId = _currentUser.OrganizationId!.Value;

            return await _context.ARPayments
                .Include(p => p.ARInvoice)
                .Where(p => p.OrganizationId == orgId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        // =========================
        // GET BY INVOICE
        // =========================
        public async Task<List<ARPayment>> GetByInvoiceAsync(Guid invoiceId)
        {
            return await _context.ARPayments
                .Where(p => p.ARInvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
    }
}