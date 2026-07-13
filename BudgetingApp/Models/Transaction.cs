

namespace BudgetingApp.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public DateOnly Date { get; set; }
        public required string Merchant { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
