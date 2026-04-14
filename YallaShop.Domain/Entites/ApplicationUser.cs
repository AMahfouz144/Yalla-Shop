using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Domain.Entites
{
    public class ApplicationUser:IdentityUser
    {
        public required string FullName { get; set; }
        public string? Address { get; set; }
        [ForeignKey("Cart")]
       // public int CartId { get; set; }
      //  public Cart Cart { get; set; }
        //the orders that the user made
        public ICollection<Order> Orders { get; set; }
        //the reviews that the user added to the products
        public ICollection<Review> Reviews { get; set; }
        //the products that the user added to his wishlist
        public ICollection<Wishlist> Wishlists { get; set; }
    }
}
