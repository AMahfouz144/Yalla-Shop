using System.ComponentModel.DataAnnotations;

namespace YallaShop.API.ViewModels.Cart
{
    public class AddToCartRequestVm
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }
}
