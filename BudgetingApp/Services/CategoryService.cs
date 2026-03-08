using BudgetingApp.Data;

namespace BudgetingApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly Dictionary<string, int> _categoryMap;

        public CategoryService(AppDbContext context)
        {
            _categoryMap = context.Categories.ToDictionary(c => c.Name.ToLowerInvariant(), c => c.Id);
        }

        public int GetCategoryIdByName(string name)
        {
            return _categoryMap[name.ToLowerInvariant()];
        }
    }
}
