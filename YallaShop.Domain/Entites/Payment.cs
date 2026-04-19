using System;
using System.ComponentModel.DataAnnotations.Schema;
using YallaShop.Domain.Enums;

namespace YallaShop.Domain.Entites
{
    public class Payment : BaseEntity
    {
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
        public string? StripeClientSecret { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
