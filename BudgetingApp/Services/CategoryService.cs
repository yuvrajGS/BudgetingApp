using BudgetingApp.Data;
using BudgetingApp.Models;
using BudgetingApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IMLService _mlService;

    private readonly ConcurrentDictionary<string, int> _categoryMap;

    public CategoryService(AppDbContext context, IMLService mlService)
    {
        _context = context;
        _mlService = mlService;

        _categoryMap = new ConcurrentDictionary<string, int>(
            context.Categories
                .AsNoTracking()
                .ToDictionary(
                    c => Normalize(c.Name),
                    c => c.Id
                )
        );
    }

    private static string Normalize(string name)
        => name.Trim().ToLowerInvariant();

    // ✅ Safe lookup (preferred for most cases)
    public bool TryGetCategoryId(string name, out int categoryId)
    {
        return _categoryMap.TryGetValue(Normalize(name), out categoryId);
    }

    // ✅ Strict lookup (throws if not found)
    public int GetCategoryIdByName(string name)
    {
        var normalized = Normalize(name);

        if (_categoryMap.TryGetValue(normalized, out var id))
            return id;

        throw new KeyNotFoundException($"Category '{name}' not found.");
    }

    // ✅ Explicit creation (ONLY place where categories are added)
    public async Task<int> CreateCategoryAsync(string name, string description, string keywords)
    {
        var normalized = Normalize(name);

        // Check cache first (avoid duplicates)
        if (_categoryMap.TryGetValue(normalized, out var existingId))
            return existingId;

        // Double-check DB (handles race conditions across instances)
        var existing = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name.Trim().ToLowerInvariant() == normalized);

        if (existing != null)
        {
            _categoryMap[normalized] = existing.Id;
            return existing.Id;
        }

        // Create new category
        var newCategory = new Category
        {
            // Adjust if you're using int PK instead of Guid
            Name = name,
            Description = description,
            Keywords = keywords,
        };

        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync();

        // Update cache
        _categoryMap[name] = newCategory.Id;

        // 🔥 Notify ML service to refresh its cache
        await _mlService.InvalidateCategoryCacheAsync();

        return newCategory.Id;
    }

    // Optional: reload entire cache
    public async Task RefreshCacheAsync()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync();

        _categoryMap.Clear();

        foreach (var c in categories)
        {
            _categoryMap[Normalize(c.Name)] = c.Id;
        }
    }
}