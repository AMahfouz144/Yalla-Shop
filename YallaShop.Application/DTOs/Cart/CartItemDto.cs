using YallaShop.Application.DTOs;

namespace YallaShop.Application.DTOs.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public ProductResponseDto? Product { get; set; }
    }
}
