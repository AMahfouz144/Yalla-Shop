namespace YallaShop.Application.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IReadOnlyList<CartItemDto> Items { get; set; } = Array.Empty<CartItemDto>();
        public decimal Subtotal { get; set; }
    }
}
