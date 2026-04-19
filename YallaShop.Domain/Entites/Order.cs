using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using YallaShop.Domain.Enums;

namespace YallaShop.Domain.Entites
{
    public class Order : BaseEntity
    {
        public OrderStatus Status { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal TotalPrice { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey("ShippingAddress")]
        public int ShippingAddressId { get; set; }
        public ShippingAddress ShippingAddress { get; set; } = null!;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public Payment? Payment { get; set; }
    }
}
