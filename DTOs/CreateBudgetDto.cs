namespace FMAS.API.DTOs
{
    public class CreateBudgetDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<CreateBudgetLineDto> Lines { get; set; } = new();
    }
}
