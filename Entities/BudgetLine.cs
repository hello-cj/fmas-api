namespace FMAS.API.Entities
{
    public class BudgetLine
    {
        public Guid BudgetLineId { get; set; }

        public Guid BudgetId { get; set; }

        public Guid AccountId { get; set; }

        public decimal Amount { get; set; } // planned amount

        public Budget Budget { get; set; }
    }
}
