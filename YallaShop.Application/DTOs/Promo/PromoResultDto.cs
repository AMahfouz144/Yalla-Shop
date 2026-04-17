namespace YallaShop.Application.DTOs.Promo
{
    public class PromoResultDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
