namespace BudgetingApp.Models
{
    public class MerchantAlias
    {
        public int Id { get; set; }
        public required string RawName { get; set; }
        public required string Category { get; set; }
    }
}
