using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Domain.Entites
{
    public  class Seller:BaseEntity
    {
        [ForeignKey("User")]    
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string? StoreName { get; set; }

        public ICollection<Product> Products { get; set; }

    }
}
