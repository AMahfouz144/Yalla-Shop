using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Application.DTOs
{
     public class WishlistRequestDto
     {
         public string UserId { get; set; }
         public int ProductId { get; set; }
     }
}
