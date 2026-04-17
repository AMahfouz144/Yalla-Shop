namespace YallaShop.Application.DTOs.Cart
{
    public class AddToCartDto
    {
        public string? UserId { get; set; }
        public string? GuestSessionId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
