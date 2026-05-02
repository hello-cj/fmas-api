namespace FMAS.API.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalJournalEntries { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public int TotalUsers { get; set; }
    }
}
