namespace FMAS.API.DTOs
{
    public class TrialBalanceDto
    {
        public string AccountName { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Balance { get; set; }
    }
}
