using Microsoft.Extensions.Configuration;
using Stripe;
using YallaShop.Application.IRepositories;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;
using YallaShop.Domain.Enums;

namespace YallaShop.Infrastructure.Services
{
    public class StripeService : IStripeService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IConfiguration _config;

        public StripeService(IGenericRepository<Order> orderRepo, IConfiguration config)
        {
            _orderRepo = orderRepo;
            _config = config;
        }

        public async Task<string> CreatePaymentIntentAsync(decimal amount, int orderId)
        {
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = "usd",
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", orderId.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);
            return intent.ClientSecret;
        }

        public async Task HandleWebhookAsync(string json, string signature)
        {
            var secret = _config["Stripe:WebhookSecret"];
            var stripeEvent = EventUtility.ConstructEvent(json, signature, secret);

            if (stripeEvent.Type != "payment_intent.succeeded")
            {
                return;
            }

            var intent = stripeEvent.Data.Object as PaymentIntent;
            if (intent == null || !intent.Metadata.TryGetValue("orderId", out var orderIdString) ||
                !int.TryParse(orderIdString, out var orderId))
            {
                return;
            }

            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
            {
                return;
            }

            order.Status = OrderStatus.Processing;
            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();
        }
    }
}
