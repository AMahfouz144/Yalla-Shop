using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace YallaShop.Application.DTOs
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}
