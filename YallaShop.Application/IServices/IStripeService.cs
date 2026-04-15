namespace YallaShop.Application.IServices
{
    public interface IStripeService
    {
        Task<string> CreatePaymentIntentAsync(decimal amount, int orderId);
        Task HandleWebhookAsync(string json, string signature);
    }
}
