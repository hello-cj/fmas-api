namespace FMAS.API.DTOs
{
    public class LedgerEntryDto
    {
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public decimal Balance { get; set; }
    }
}
