namespace YallaShop.Application.DTOs.Checkout
{
    public class CheckoutDto
    {
        public string? UserId { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestSessionId { get; set; }
        public int ShippingAddressId { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string? PromoCode { get; set; }
    }
}
