namespace YallaShop.Application.DTOs.Cart
{
    public class UpdateCartItemDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public string? UserId { get; set; }
        public string? GuestSessionId { get; set; }
    }
}
