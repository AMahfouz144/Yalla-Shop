using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Domain.Entites
{
    public class Cart:BaseEntity
    {
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public string? GuestSessionId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
   
    }
}
