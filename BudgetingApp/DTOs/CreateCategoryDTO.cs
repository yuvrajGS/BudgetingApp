namespace BudgetingApp.DTOs
{
    public class CreateCategoryDTO
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Keywords { get; set; }
    }
}
