using BudgetingApp.DTOs;
using BudgetingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> UploadPdf([FromForm] DocumentUploadDTO dto)
        {

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file uploaded.");

            if (dto.File.ContentType != "application/pdf")
                return BadRequest("Only PDF files are allowed.");

            using var stream = dto.File.OpenReadStream();

            var transactions = await _documentService.ProcessPdfAsync(dto.UserId, stream);

            return Ok(transactions);
        }
    }
}
