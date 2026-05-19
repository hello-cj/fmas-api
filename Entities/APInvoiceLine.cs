using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMAS.API.Entities
{
    [Table("ap_invoice_lines")]
    public class APInvoiceLine
    {
        [Key]
        public Guid APInvoiceLineId { get; set; }

        public Guid APInvoiceId { get; set; }

        public Guid AccountId { get; set; }

        public string Description { get; set; } = null!;

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total { get; set; }
        //public decimal Total => Quantity * UnitPrice;

        public APInvoice APInvoice { get; set; } = null!;
    }
}