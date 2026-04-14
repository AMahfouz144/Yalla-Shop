using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.IRepositories;
using YallaShop.Domain.Entites;
using YallaShop.Infrastructure.Persistence;

namespace YallaShop.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly AppDbContext dbContext;

        public ReviewRepository(AppDbContext appDbContext) : base(appDbContext)
        {

            dbContext = appDbContext;
        }

        public IQueryable<Review> GetAllByProductId(int productId)
        {
            return dbContext.Reviews.Where(r => r.ProductId == productId && !r.IsDeleted);
        }
    }
}
