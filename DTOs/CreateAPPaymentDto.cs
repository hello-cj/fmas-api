namespace FMAS.API.DTOs
{
    public class CreateAPPaymentDto
    {
        public Guid APInvoiceId { get; set; }

        public Guid VendorId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Reference { get; set; }
    }
}
