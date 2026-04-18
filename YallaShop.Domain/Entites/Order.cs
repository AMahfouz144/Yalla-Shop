using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Enums;

namespace YallaShop.Domain.Entites
{
    public  class Order: BaseEntity
    {
        public OrderStatus Status { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal TotalPrice { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<OrderItem> Items { get; set; }

        public Payment Payment { get; set; }


        // --- Shipping Address Fields --- 
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        // --- Payment Information ---
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
    }
}

/*
 
 using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Enums;

namespace YallaShop.Domain.Entites
{
    public class Order : BaseEntity
    {
        public OrderStatus Status { get; set; }
        
        [Column(TypeName = "decimal(18,6)")]
        public decimal TotalPrice { get; set; }
        
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<OrderItem> Items { get; set; }

        public Payment Payment { get; set; }

        // --- Shipping Address Fields --- 
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        
        // --- Payment Information ---
        public string PaymentMethod { get; set; } 
        public bool IsPaid { get; set; } 
    }
}

 
 
 */
