using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Enums;

namespace YallaShop.Application.DTOs.User
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }

        public ProductStatus Status { get; set; } // Pending , Accepted, Rejected By Admin?
        public int CategoryId { get; set; }

        public int? SellerId { get; set; }
    }
}
