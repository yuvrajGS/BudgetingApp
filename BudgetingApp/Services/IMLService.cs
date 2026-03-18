namespace BudgetingApp.Services
{
    public interface IMLService
    {
        Task<string> PredictCategoryAsync(string merchant);

        Task<List<string>> PredictCategoryBatchAsync(List<string> merchants);

        Task InvalidateCategoryCacheAsync();


    }
}
