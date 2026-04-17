using System.ComponentModel.DataAnnotations.Schema;
using YallaShop.Domain.Enums;

namespace YallaShop.Domain.Entites
{
    public class PromoCode : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? MinOrderAmount { get; set; }

        public int? MaxUsageCount { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
