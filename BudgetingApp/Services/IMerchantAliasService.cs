namespace BudgetingApp.Services
{
    public interface IMerchantAliasService
    {
        Task<string?> GetMerchantAliasById(int Id);

        Task<string?> GetMerchantAliasByRawName(string RawName);

    }
}
