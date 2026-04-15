using System.ComponentModel.DataAnnotations;

namespace YallaShop.API.ViewModels
{
    public class RegisterViewModel
    {
        [MaxLength(50)]
        public required string FullName { get; set; }

        [EmailAddress]
        public required string UserName { get; set; }

        [Length(7, 20)]

        public required string Password { get; set; }

        public string Role { get; set; }
        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? StoreName { get; set; }
    }
}
