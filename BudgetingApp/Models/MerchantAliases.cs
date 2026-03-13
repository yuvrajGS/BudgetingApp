namespace BudgetingApp.Models
{
    public class MerchantAliases
    {
        public int Id { get; set; }
        public required string RawName { get; set; }
        public required string CleanName { get; set; }
    }
}
