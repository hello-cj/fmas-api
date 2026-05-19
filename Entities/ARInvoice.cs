using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMAS.API.Entities
{
    [Table("ar_invoices")]
    public class ARInvoice
    {
        [Key]
        public Guid ARInvoiceId { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid CustomerId { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }
        public string Description { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; } = 0;

        public InvoiceStatus Status { get; set; }

        public Customer Customer { get; set; } = null!;

        public ICollection<ARInvoiceLine> Lines { get; set; } = new List<ARInvoiceLine>();
    }
}