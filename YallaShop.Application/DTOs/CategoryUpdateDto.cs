using System.ComponentModel.DataAnnotations;

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
