namespace YallaShop.Application.IServices
{
    public interface IStripeService
    {
        Task<(string ClientSecret, string? PaymentIntentId)> CreatePaymentIntentAsync(decimal amount, int orderId);
        Task HandleWebhookAsync(string json, string signature);
    }
}
