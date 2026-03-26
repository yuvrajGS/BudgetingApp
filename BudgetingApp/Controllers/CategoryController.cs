using BudgetingApp.DTOs;
using BudgetingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories()
        {
            var categories = await _service.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<int>> GetCategoryIdByName(string name)
        {
            try
            {
                var id = _service.GetCategoryIdByName(name);
                return Ok(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateCategory(CreateCategoryDTO dto)
        {
            try
            {
                var id = await _service.CreateCategoryAsync(dto);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
