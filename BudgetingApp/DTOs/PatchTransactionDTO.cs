namespace BudgetingApp.DTOs
{
    public class PatchTransactionDTO
    {
        public DateOnly? Date { get; set; }
        public string? Merchant { get; set; }
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
    }
}
