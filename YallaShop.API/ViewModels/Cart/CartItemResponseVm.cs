namespace YallaShop.API.ViewModels.Cart
{
    public class CartItemResponseVm
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public ProductViewModel? Product { get; set; }
    }
}
