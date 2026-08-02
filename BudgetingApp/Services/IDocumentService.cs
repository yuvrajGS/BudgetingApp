using BudgetingApp.DTOs;

namespace BudgetingApp.Services

{
    public interface IDocumentService
    {
        Task<IEnumerable<CreateTransactionDTO>> ProcessPdfAsync(Guid userId, Stream pdfStream);
    }
}
