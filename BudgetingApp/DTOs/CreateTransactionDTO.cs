namespace BudgetingApp.DTOs
{
    public class CreateTransactionDTO
    {
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public required string Merchant { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }

    }
}
