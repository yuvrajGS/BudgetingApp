using BudgetingApp.Data;
using BudgetingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Services
{
    public class MerchantAliasService : IMerchantAliasService
    {

        private readonly AppDbContext _context;
        public MerchantAliasService(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<Boolean> MerchantAliasExists(string alias)
        {
            return await _context.MerchantAlias.AnyAsync(ma => ma.RawName.ToLower() == alias.ToLower());
        }

        public async Task AddMerchantAlias(string rawName, string categoryName)
        {
            var alias = new MerchantAlias
            {
                RawName = rawName,
                Category = categoryName
            };
            _context.MerchantAlias.Add(alias);
            await _context.SaveChangesAsync();
        }

        public void RemoveMerchantAlias(string rawName)
        {
            var alias = _context.MerchantAlias.FirstOrDefault(ma => ma.RawName.ToLower() == rawName.ToLower());
            if (alias != null)
            {
                _context.MerchantAlias.Remove(alias);
                _context.SaveChanges();
            }
        }

        public async Task ChangeMerchantAliasCategory(string rawName, string newCategory)
        {
            var alias = await _context.MerchantAlias.FirstOrDefaultAsync(ma => ma.RawName.ToLower() == rawName.ToLower());
            if (alias != null)
            {
                alias.Category = newCategory;
                await _context.SaveChangesAsync();
            }
        }
    }
}