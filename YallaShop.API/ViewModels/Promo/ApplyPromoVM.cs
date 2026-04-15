using System.ComponentModel.DataAnnotations;

namespace YallaShop.API.ViewModels.Promo
{
    public class ApplyPromoVM
    {
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
