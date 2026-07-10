using BudgetingApp.DTOs;
using BudgetingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TransactionDTO>> GetById(Guid id)
        {
            var transaction = await _service.GetTransactionByIdAsync(id);
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDTO>> CreateTransaction(CreateTransactionDTO dto)
        {
            var transaction = await _service.CreateTransactionAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> CreateTransactionsBatch(IEnumerable<CreateTransactionDTO> dtos)
        {
            var transactions = await _service.CreateTransactionsBatchAsync(dtos);
            return Ok(transactions);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetByUser(Guid userId)
        {
            var transactions = await _service.GetTransactionsByUserAsync(userId);
            return Ok(transactions);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteTransaction(Guid id)
        {
            try
            {
                await _service.DeleteTransactionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<TransactionDTO>> UpdateTransaction(Guid id, UpdateTransactionDTO dto)
        {
            try
            {
                var updatedTransaction = await _service.UpdateTransactionAsync(id, dto);
                return Ok(updatedTransaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
