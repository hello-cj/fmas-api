namespace FMAS.API.DTOs
{
    public class CreateAPInvoiceLineDto
    {
        public Guid AccountId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}