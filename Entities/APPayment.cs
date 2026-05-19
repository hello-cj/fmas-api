using System.ComponentModel.DataAnnotations;

namespace FMAS.API.Entities
{
    public class APPayment
    {
        [Key]
        public Guid APPaymentId { get; set; }

        public Guid APInvoiceId { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid VendorId { get; set; }

        public Vendor Vendor { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Reference { get; set; }

        public APInvoice APInvoice { get; set; }

        public ICollection<APPaymentAllocation> Allocations { get; set; }
    }
}