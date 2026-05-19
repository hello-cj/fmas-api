namespace FMAS.API.DTOs
{
    public class CreateAPInvoiceDto
    {
        public Guid VendorId { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }

        public List<CreateAPInvoiceLineDto> Lines { get; set; } = new();
    }
}