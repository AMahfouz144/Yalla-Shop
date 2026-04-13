using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Entites;

namespace YallaShop.Application.IRepositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
    }
}
