using Microsoft.AspNetCore.Mvc;
using YallaShop.Application.DTOs;
using YallaShop.Application.IServices;

namespace YallaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISellerService _sellerService;


        public ProductController(IProductService productService, ICategoryService categoryService, ISellerService sellerService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _sellerService = sellerService;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/Product/filter?name=x&minPrice=10&sortBy=price&sortOrder=asc
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Filter([FromQuery] ProductFilterDto filter)
        {
            var products = await _productService.FilterAsync(filter);
            return Ok(products);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { Message = $"Product with Id {id} not found." });

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] ProductAddDto dto)
        {
            if(await _categoryService.GetByIdAsync(dto.CategoryId) == null)
                return BadRequest(new { Message = $"Category with Id {dto.CategoryId} does not exist." });

            if (dto.SellerId.HasValue && !await _sellerService.GetSellerByIdAsync(dto.SellerId.Value))
                return BadRequest(new { Message = $"Seller with Id {dto.SellerId.Value} does not exist." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            if (await _categoryService.GetByIdAsync(dto.CategoryId) == null)
                return BadRequest(new { Message = $"Category with Id {dto.CategoryId} does not exist." });

            if (dto.SellerId.HasValue && !await _sellerService.GetSellerByIdAsync(dto.SellerId.Value))
                return BadRequest(new { Message = $"Seller with Id {dto.SellerId.Value} does not exist." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest(new { Message = "Id in URL does not match Id in request body." });

            var product = await _productService.UpdateAsync(dto);
            if (product == null)
                return NotFound(new { Message = $"Product with Id {id} not found." });

            return Ok(product);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
                return NotFound(new { Message = $"Product with Id {id} not found." });

            return Ok(new { Message = "Deleted" });
        }
    }
}
