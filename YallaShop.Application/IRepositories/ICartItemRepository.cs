using YallaShop.Domain.Entites;

namespace YallaShop.Application.IRepositories
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        Task<CartItem?> GetByCartIdAndProductIdAsync(int cartId, int productId);
        Task<CartItem?> GetByIdWithDetailsAsync(int cartItemId);
    }
}
