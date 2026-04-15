using System.ComponentModel.DataAnnotations;

namespace YallaShop.API.ViewModels.Promo
{
    public class CreatePromoVM
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string DiscountType { get; set; } = string.Empty;
        [Range(1, 100)]
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? MaxUsageCount { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
