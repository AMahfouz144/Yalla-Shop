using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using YallaShop.Domain.Enums;

namespace YallaShop.Application.DTOs
{
    public class ProductUpdateDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }


        public IFormFile? Image { get; set; }

        public ProductStatus Status { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int? SellerId { get; set; }
    }
}
