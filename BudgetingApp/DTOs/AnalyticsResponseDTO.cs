namespace BudgetingApp.DTOs
{
    public class AnalyticsResponseDTO
    {
        public Dictionary<string, decimal> CategoryBreakdown { get; set; }
        public List<MonthlySpendingDTO> MonthlySpending { get; set; }
        public List<TopMerchantDTO> TopMerchants { get; set; }

    }

    public class MonthlySpendingDTO
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }

    public class TopMerchantDTO
    {
        public string Merchant { get; set; }
        public decimal Amount { get; set; }
    }

}
