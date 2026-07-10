using BudgetingApp.DTOs;

namespace BudgetingApp.Services
{
    public interface ITransactionService
    {
        Task<TransactionDTO> GetTransactionByIdAsync(Guid Id);
        Task<TransactionDTO> CreateTransactionAsync(CreateTransactionDTO dto);
        Task<IEnumerable<TransactionDTO>> CreateTransactionsBatchAsync(IEnumerable<CreateTransactionDTO> dtos);
        Task<IEnumerable<TransactionDTO>> GetTransactionsByUserAsync(Guid userId);
        Task DeleteTransactionAsync(Guid id);
        Task<TransactionDTO> UpdateTransactionAsync(Guid id, UpdateTransactionDTO dto);
    }
}
