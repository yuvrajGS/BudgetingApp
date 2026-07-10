using BudgetingApp.Data;
using BudgetingApp.DTOs;
using BudgetingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IMLService _mlService;
        private readonly ICategoryService _categoryService;
        private readonly AppDbContext _context;
        private readonly IMerchantAliasService _merchantAliasService;

        public TransactionService(AppDbContext context, IMLService mLService, ICategoryService categoryService, IMerchantAliasService merchantAliasService)
        {
            _context = context;
            _mlService = mLService;
            _categoryService = categoryService;
            _merchantAliasService = merchantAliasService;
        }
         
        public async Task<TransactionDTO> GetTransactionByIdAsync(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }
            return MapToDTO(transaction);
        }

        public async Task<TransactionDTO> CreateTransactionAsync(CreateTransactionDTO dto)
        {
            // Call ML service for category
            var categoryTuple = await _mlService.PredictCategoryAsync(dto.Merchant);


            var categoryId = _categoryService.GetCategoryIdByName(categoryTuple.category);

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Date = dto.Date,
                Merchant = categoryTuple.cleanName,
                Amount = dto.Amount,
                Description = "Merchant: " + dto.Merchant + " | " + dto.Description,
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return MapToDTO(transaction);
        }

        public async Task<IEnumerable<TransactionDTO>> CreateTransactionsBatchAsync(IEnumerable<CreateTransactionDTO> dtos)
        {
            // Batch descriptions and Merchant for ML
            var merchant = dtos.Select(d => d.Merchant).ToList();
            var categories = await _mlService.PredictCategoryBatchAsync(merchant);

            for (int i = 0; i < categories.Count(); i++)
            {
                if (string.IsNullOrEmpty(categories[i].category))
                {
                    categories[i] = ("Uncategorized", categories[i].cleanName); // Default category
                }
            }

            var transactions = dtos.Select((dto, i) => new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Date = dto.Date,
                Merchant = categories[i].cleanName,
                Amount = dto.Amount,
                Description = "Merchant: " + dto.Merchant + " | " + dto.Description,
                CategoryId = _categoryService.GetCategoryIdByName(categories[i].category),
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync();
            return transactions.Select(MapToDTO);
        }

        public async Task<IEnumerable<TransactionDTO>> GetTransactionsByUserAsync(Guid userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();
            return transactions.Select(MapToDTO);
        }

        public async Task DeleteTransactionAsync(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<TransactionDTO> UpdateTransactionAsync(Guid id, UpdateTransactionDTO dto)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }
            transaction.Date = dto.Date;
            transaction.Merchant = dto.Merchant;
            transaction.Amount = dto.Amount;
            transaction.Description = dto.Description;

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null){
                throw new KeyNotFoundException("Category not found");
            }

            transaction.CategoryId = dto.CategoryId;
            var merchantExists = await _merchantAliasService.MerchantAliasExists(dto.Merchant);
            if (merchantExists)
            {
                await _merchantAliasService.ChangeMerchantAliasCategory(dto.Merchant, category.Name);
            }
            else
            {
                await _merchantAliasService.AddMerchantAlias(dto.Merchant, category.Name);
            }

            await _context.SaveChangesAsync();
            return MapToDTO(transaction);
        }

        private TransactionDTO MapToDTO(Transaction t) =>
            new TransactionDTO
            {
                Id = t.Id,
                UserId = t.UserId,
                Date = t.Date,
                Merchant = t.Merchant,
                Amount = t.Amount,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CreatedAt = t.CreatedAt
            };
    }
}
