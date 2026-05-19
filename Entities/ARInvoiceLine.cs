using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMAS.API.Entities
{
    [Table("ar_invoice_lines")]
    public class ARInvoiceLine
    {
        [Key]
        public Guid ARInvoiceLineId { get; set; }

        public Guid ARInvoiceId { get; set; }

        public Guid AccountId { get; set; }

        public string Description { get; set; } = null!;

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total { get; set; }
        //public decimal Total => Quantity * UnitPrice;

        public ARInvoice ARInvoice { get; set; } = null!;
    }
}