using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.IRepositories;
using YallaShop.Domain.Entites;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Repositories
{
 public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
 {
    private readonly AppDbContext dbContext;

    public WishlistRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        dbContext = appDbContext;
    }

        public async Task<Wishlist> GetByUserIdAndProductIdAsync(string userId, int productId)
        {
            return await dbContext.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDeleted);
        }
    }
}
