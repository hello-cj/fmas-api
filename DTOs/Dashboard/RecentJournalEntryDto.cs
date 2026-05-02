namespace FMAS.API.DTOs.Dashboard
{
    public class RecentJournalEntryDto
    {
        public Guid JournalEntryId { get; set; }
        public DateTime Date { get; set; }
        public string? Reference { get; set; }
        public string? Description { get; set; }

        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
    }
}
