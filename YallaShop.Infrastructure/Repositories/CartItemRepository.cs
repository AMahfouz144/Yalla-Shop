using Microsoft.EntityFrameworkCore;
using YallaShop.Application.IRepositories;
using YallaShop.Domain.Entites;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Repositories
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        private readonly AppDbContext _dbContext;

        public CartItemRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _dbContext = appDbContext;
        }

        public Task<CartItem?> GetByCartIdAndProductIdAsync(int cartId, int productId)
        {
            return _dbContext.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId && !ci.IsDeleted);
        }

        public Task<CartItem?> GetByIdWithDetailsAsync(int cartItemId)
        {
            return _dbContext.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && !ci.IsDeleted);
        }
    }
}
