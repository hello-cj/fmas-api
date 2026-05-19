namespace FMAS.API.DTOs
{
    public class CreateARPaymentDto
    {
        public Guid ARInvoiceId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Reference { get; set; }
    }
}
