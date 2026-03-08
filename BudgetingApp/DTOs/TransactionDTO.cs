namespace BudgetingApp.DTOs
{
    public class TransactionDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public required string Merchant { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
