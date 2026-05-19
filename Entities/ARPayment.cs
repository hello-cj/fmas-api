using FMAS.API.Entities;

public class ARPayment
{
    public Guid ARPaymentId { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }

    public Guid ARInvoiceId { get; set; } 

    public ARInvoice ARInvoice { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string Reference { get; set; }

    public Guid OrganizationId { get; set; }

    

    public ICollection<ARPaymentAllocation> Allocations { get; set; }
}