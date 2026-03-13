namespace BudgetingApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Keywords { get; set; } // Denormalized list of keywords for ML Service only
        public DateTime CreatedAt { get; set; }


        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}