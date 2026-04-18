using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace YallaShop.Application.DTOs
{
    public class CategoryAddDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}
