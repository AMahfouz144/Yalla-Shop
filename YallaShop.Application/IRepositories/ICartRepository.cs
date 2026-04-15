using YallaShop.Domain.Entites;

namespace YallaShop.Application.IRepositories
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartWithItemsAsync(string? userId, string? guestSessionId);
        Task ClearCartAsync(string? userId, string? guestSessionId);
    }
}
