namespace FMAS.API.Entities
{
    public class APPaymentAllocation
    {
        public Guid APPaymentAllocationId { get; set; }

        public Guid APPaymentId { get; set; }

        public Guid APInvoiceId { get; set; }

        public decimal Amount { get; set; }
    }
}
