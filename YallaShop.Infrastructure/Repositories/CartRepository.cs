using Microsoft.EntityFrameworkCore;
using YallaShop.Application.IRepositories;
using YallaShop.Domain.Entites;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly AppDbContext _dbContext;

        public CartRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _dbContext = appDbContext;
        }

        public Task<Cart?> GetCartWithItemsAsync(string? userId, string? guestSessionId)
        {
            return _dbContext.Carts
                .AsSplitQuery()
                .Include(c => c.CartItems.Where(ci => !ci.IsDeleted))
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => !c.IsDeleted &&
                    ((userId != null && c.UserId == userId) ||
                     (guestSessionId != null && c.GuestSessionId == guestSessionId)));
        }

        public async Task ClearCartAsync(string? userId, string? guestSessionId)
        {
            var cart = await GetCartWithItemsAsync(userId, guestSessionId);
            if (cart == null || cart.CartItems.Count == 0)
            {
                return;
            }

            _dbContext.CartItems.RemoveRange(cart.CartItems);
            await _dbContext.SaveChangesAsync();
        }
    }
}
