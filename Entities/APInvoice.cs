using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMAS.API.Entities
{
    [Table("ap_invoices")]
    public class APInvoice
    {
        [Key]
        public Guid APInvoiceId { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid VendorId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string Reference { get; set; } = null!;

        public string Description { get; set; } = null!;

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        public decimal TotalAmount { get; set; }
        public Vendor Vendor { get; set; } = null!;

        public ICollection<APInvoiceLine> Lines { get; set; } = new List<APInvoiceLine>();
    }
}