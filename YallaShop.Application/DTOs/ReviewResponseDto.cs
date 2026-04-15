using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Application.DTOs
{
     public class ReviewResponseDto
     {
             public int Id { get; set; }
             public string UserId { get; set; }
             public int ProductId { get; set; }
             public int Rating { get; set; }
             public string? Comment { get; set; }
             public DateTime CreatedAt { get; set; }
     }
}
