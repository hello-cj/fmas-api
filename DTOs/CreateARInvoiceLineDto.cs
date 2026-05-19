namespace FMAS.API.DTOs
{
    public class CreateARInvoiceLineDto
    {
        public Guid AccountId { get; set; }
        public string Description { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
