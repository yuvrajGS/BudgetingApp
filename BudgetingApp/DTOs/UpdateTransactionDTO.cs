namespace BudgetingApp.DTOs
{
    public class UpdateTransactionDTO
    {
        public required DateOnly Date { get; set; }
        public required string Merchant { get; set; }
        public required decimal Amount { get; set; }
        public required string Description { get; set; }
        public required int CategoryId { get; set; }
    }
}
