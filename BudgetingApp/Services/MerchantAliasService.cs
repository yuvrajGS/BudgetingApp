using BudgetingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Services
{
    public class MerchantAliasService : IMerchantAliasService
    {
        private readonly AppDbContext _context;

        public MerchantAliasService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetMerchantAliasById(int Id)
        {
            var alias = await _context.MerchantAlias.FindAsync(Id);

            return alias?.CleanName;
        }

        public async Task<string?> GetMerchantAliasByRawName(string RawName)
        {
            var CleanName = await _context.MerchantAlias
                .Where(a => a.RawName == RawName)
                .Select(a => a.CleanName)
                .FirstOrDefaultAsync();

            return CleanName;
        }
    }
}
