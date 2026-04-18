using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Enums;

namespace YallaShop.Domain.Entites
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public byte[]? Picture { get; set; }

        public ProductStatus Status { get; set; } // Pending , Accepted, Rejected By Admin?
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int? SellerId { get; set; }
        public Seller? Seller { get; set; }

        public ICollection<Review> Reviews { get; set; }

        //in case we want to know in which carts this product is included
        public ICollection<CartItem> CartItems { get; set; }

        //in case we want to know in which orders this product is included
        public ICollection<OrderItem> OrderItems { get; set; }


    }
}
