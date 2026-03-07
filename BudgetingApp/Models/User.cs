namespace BudgetingApp.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();



    }
}
