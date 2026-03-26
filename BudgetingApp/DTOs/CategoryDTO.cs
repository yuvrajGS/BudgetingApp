namespace BudgetingApp.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Keywords { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
