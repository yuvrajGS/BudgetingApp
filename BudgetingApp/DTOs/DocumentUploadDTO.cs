namespace BudgetingApp.DTOs
{
    public class DocumentUploadDTO
    {
        public Guid UserId { get; set; }
        public IFormFile File { get; set; } = null!;

    }
}
