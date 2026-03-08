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
        private readonly HttpClient _mlClient;

        public TransactionService(AppDbContext context, HttpClient mlClient, IMLService mLService, ICategoryService categoryService)
        {
            _context = context;
            _mlClient = mlClient;
            _mlService = mLService;
            _categoryService = categoryService;
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
            var categoryName = await _mlService.PredictCategoryAsync(dto.Merchant);


            var categoryId = _categoryService.GetCategoryIdByName(categoryName);

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Date = dto.Date,
                Merchant = dto.Merchant,
                Amount = dto.Amount,
                Description = dto.Description,
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
                if (string.IsNullOrEmpty(categories[i]))
                {
                    categories[i] = "Uncategorized"; // Default category
                }
            }

            var transactions = dtos.Select((dto, i) => new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Date = dto.Date,
                Merchant = dto.Merchant,
                Amount = dto.Amount,
                Description = dto.Description,
                CategoryId = _categoryService.GetCategoryIdByName(categories[i]),
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
