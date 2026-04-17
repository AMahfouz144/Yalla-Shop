namespace YallaShop.API.ViewModels.Cart
{
    public class CartResponseVm
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IReadOnlyList<CartItemResponseVm> Items { get; set; } = Array.Empty<CartItemResponseVm>();
        public decimal Subtotal { get; set; }
    }
}
