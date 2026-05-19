using FMAS.API.Entities;

public class ARPaymentAllocation
{
    public Guid ARPaymentAllocationId { get; set; }

    public Guid ARPaymentId { get; set; }
    public ARPayment ARPayment { get; set; }

    public Guid ARInvoiceId { get; set; }
    public ARInvoice ARInvoice { get; set; }

    public decimal Amount { get; set; }
}