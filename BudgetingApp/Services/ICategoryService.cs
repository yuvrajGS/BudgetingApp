using BudgetingApp.DTOs;

namespace BudgetingApp.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        int GetCategoryIdByName(string name);
        Task<int> CreateCategoryAsync(CreateCategoryDTO dto);
    }
}
