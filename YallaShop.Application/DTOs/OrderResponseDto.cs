using YallaShop.Application.DTOs.ShippingAddress;
using YallaShop.Domain.Enums;

namespace YallaShop.Application.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        public ShippingAddressDto? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }
}
