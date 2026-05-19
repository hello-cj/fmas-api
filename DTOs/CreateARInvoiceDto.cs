namespace FMAS.API.DTOs
{
    public class CreateARInvoiceDto
    {
        public Guid CustomerId { get; set; }

        public string InvoiceNumber { get; set; }  = string.Empty;

        public DateTime InvoiceDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public List<CreateARInvoiceLineDto> Lines { get; set; }
    }
}
