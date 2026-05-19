namespace FMAS.API.Entities
{
    public class Budget
    {
        public Guid BudgetId { get; set; }

        public Guid OrganizationId { get; set; }

        public string Name { get; set; } = string.Empty; // e.g. "2026 Operating Budget"

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<BudgetLine> Lines { get; set; } = new();
    }
}
