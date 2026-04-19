using YallaShop.Application.DTOs.ShippingAddress;

namespace YallaShop.Application.DTOs.Checkout
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? StripeClientSecret { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; } = null!;
    }
}
