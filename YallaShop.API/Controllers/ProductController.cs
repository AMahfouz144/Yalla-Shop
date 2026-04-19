using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaShop.API.Helpers;
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
        private readonly IFileHelper _fileHelper;


        public ProductController(IProductService productService, ICategoryService categoryService, ISellerService sellerService, IFileHelper fileHelper)
        {
            _productService = productService;
            _categoryService = categoryService;
            _sellerService = sellerService;
            _fileHelper = fileHelper;
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

        [Authorize(Roles = "Seller")]
        [HttpGet("GetProductsofSellerid/{seller_id}")]
        public async Task<ActionResult<List<ProductDto>>> GetProducts_of_seller(int seller_id)
        {
            var product = await _productService.GetProductsOfSeller(seller_id);
            if (product == null)
                return NotFound(new { Message = $"Products with seller Id {seller_id} not found." });

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<ProductDto>> Create([FromForm] ProductAddDto dto)
        {
            if(await _categoryService.GetByIdAsync(dto.CategoryId) == null)
                return BadRequest(new { Message = $"Category with Id {dto.CategoryId} does not exist." });

            if (dto.SellerId.HasValue && !await _sellerService.GetSellerByIdAsync(dto.SellerId.Value))
                return BadRequest(new { Message = $"Seller with Id {dto.SellerId.Value} does not exist." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? imageUrl = await _fileHelper.UploadPhoto(dto.Image);

            var product = await _productService.CreateAsync(imageUrl,dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromForm] ProductUpdateDto dto)
        {
            if (await _categoryService.GetByIdAsync(dto.CategoryId) == null)
                return BadRequest(new { Message = $"Category with Id {dto.CategoryId} does not exist." });

            if (dto.SellerId.HasValue && !await _sellerService.GetSellerByIdAsync(dto.SellerId.Value))
                return BadRequest(new { Message = $"Seller with Id {dto.SellerId.Value} does not exist." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest(new { Message = "Id in URL does not match Id in request body." });

            string imageUrl = await _fileHelper.UploadPhoto(dto.Image);

            var product = await _productService.UpdateAsync(imageUrl, dto);
            if (product == null)
                return NotFound(new { Message = $"Product with Id {id} not found." });

            return Ok(product);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin,Seller")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
                return NotFound(new { Message = $"Product with Id {id} not found." });

            return Ok(new { Message = "Deleted" });
        }
    }
}
