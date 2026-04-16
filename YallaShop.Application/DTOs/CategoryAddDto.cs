using System.ComponentModel.DataAnnotations;

namespace YallaShop.Application.DTOs
{
    public class CategoryAddDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}
