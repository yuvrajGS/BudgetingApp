namespace BudgetingApp.DTOs
{
    public class TransactionUploadDTO
    {
        public DateTime Date { get; set; }
        public string Merchant { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

    }
}
