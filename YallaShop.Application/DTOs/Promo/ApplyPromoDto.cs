namespace YallaShop.Application.DTOs.Promo
{
    public class ApplyPromoDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal CartSubTotal { get; set; }
    }
}
