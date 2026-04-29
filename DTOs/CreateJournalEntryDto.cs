namespace FMAS.API.DTOs
{
    public class CreateJournalEntryDto
    {
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }

        public List<CreateJournalEntryLineDto> Lines { get; set; }
    }

    public class CreateJournalEntryLineDto
    {
        public Guid AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
