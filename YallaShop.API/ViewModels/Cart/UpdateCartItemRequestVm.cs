using System.ComponentModel.DataAnnotations;

namespace YallaShop.API.ViewModels.Cart
{
    public class UpdateCartItemRequestVm
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
