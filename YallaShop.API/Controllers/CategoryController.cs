using Microsoft.AspNetCore.Mvc;
using YallaShop.Application.DTOs;
using YallaShop.Application.IServices;

namespace YallaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { Message = $"Category with Id {id} not found." });

            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryAddDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest(new { Message = "Id in URL does not match Id in request body." });

            var category = await _categoryService.UpdateAsync(dto);
            if (category == null)
                return NotFound(new { Message = $"Category with Id {id} not found." });

            return Ok(category);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
                return NotFound(new { Message = $"Category with Id {id} not found." });

            return Ok(new { Message = "Deleted" });
        }
    }
}
