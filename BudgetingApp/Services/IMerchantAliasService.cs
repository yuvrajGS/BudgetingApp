using BudgetingApp.Models;

namespace BudgetingApp.Services
{
    public interface IMerchantAliasService
    {
        Task<Boolean> MerchantAliasExists(string alias);
        Task AddMerchantAlias(string rawName, string categoryName);
        void RemoveMerchantAlias(string rawName);
        Task ChangeMerchantAliasCategory(string rawName, string newCategory);
    }
}
