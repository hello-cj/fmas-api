namespace FMAS.API.DTOs
{
    public class CreateBudgetLineDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
