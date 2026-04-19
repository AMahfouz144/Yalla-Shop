using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using YallaShop.Application.IServices;
using YallaShop.Domain.Enums;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Services
{//hello address shipping
    public class StripeService : IStripeService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public StripeService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<(string ClientSecret, string? PaymentIntentId)> CreatePaymentIntentAsync(
            decimal amount, int orderId)
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
            return (intent.ClientSecret, intent.Id);
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

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return;
            }

            order.Status = OrderStatus.Processing;
            _context.Orders.Update(order);

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment != null)
            {
                payment.Status = PaymentStatus.Completed;
                payment.PaidAt = DateTime.UtcNow;
                payment.TransactionId = intent.Id;
                _context.Payments.Update(payment);
            }

            await _context.SaveChangesAsync();
        }
    }
}
