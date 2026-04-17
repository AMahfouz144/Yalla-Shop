using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Application.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task<bool> DeleteAsync(int id);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);

        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null);
		Task SaveChangesAsync();
	}
}
