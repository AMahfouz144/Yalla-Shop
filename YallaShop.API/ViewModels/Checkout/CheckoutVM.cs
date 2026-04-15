using System.ComponentModel.DataAnnotations;

namespace YallaShop.API.ViewModels.Checkout
{
    public class CheckoutVM
    {
        [Required]
        public int ShippingAddressId { get; set; }
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
        public string? PromoCode { get; set; }
        public string? GuestEmail { get; set; }
    }
}
