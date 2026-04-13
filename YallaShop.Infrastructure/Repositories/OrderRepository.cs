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
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        
    }
}
