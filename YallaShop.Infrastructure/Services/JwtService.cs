using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Application.IServices;
using YallaShop.Domain.Entites;

namespace YallaShop.Infrastructure.Services
{
    public class JwtService(UserManager<ApplicationUser> userManager,IConfiguration config) : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager=userManager;
        private readonly IConfiguration _config=config;
        public async Task<(string token, DateTime expiration)> GenerateTokenAsync(ApplicationUser user) {

            List<Claim> myclaims = new List<Claim>();
            myclaims.Add(new Claim(ClaimTypes.Name, user.UserName));
            myclaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            myclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            //claim  roles
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                myclaims.Add(new Claim(ClaimTypes.Role, role));
            }


            var SignKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]));

            SigningCredentials signingCredentials =
                new SigningCredentials(SignKey, SecurityAlgorithms.HmacSha256);



            //create token
            JwtSecurityToken mytoken = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],//provider create token
                audience: config["Jwt:Audience"],//cousumer url
                expires: DateTime.Now.AddHours(1),
                claims: myclaims,
                signingCredentials: signingCredentials);

            return (new JwtSecurityTokenHandler().WriteToken(mytoken), mytoken.ValidTo);
        }

    }
}
