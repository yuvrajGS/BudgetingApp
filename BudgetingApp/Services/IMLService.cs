namespace BudgetingApp.Services
{
    public interface IMLService
    {
        Task<(string category, string cleanName)> PredictCategoryAsync(string merchant);

        Task<List<(string category, string cleanName)>> PredictCategoryBatchAsync(List<string> merchants);

        Task InvalidateCategoryCacheAsync();


    }
}
