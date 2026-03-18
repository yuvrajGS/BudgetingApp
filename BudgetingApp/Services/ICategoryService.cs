namespace BudgetingApp.Services
{
    public interface ICategoryService
    {
        int GetCategoryIdByName(string name);
        Task<int> CreateCategoryAsync(string name, string description, string keywords);
    }
}
