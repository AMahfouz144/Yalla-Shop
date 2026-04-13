using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Entites;

namespace YallaShop.Application.IServices
{
    public interface IJwtService
    {
        Task<(string token, DateTime expiration)> GenerateTokenAsync(ApplicationUser user);
    }
}
