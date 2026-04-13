using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Domain.Entites
{
    public class CartItem:BaseEntity
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal TotalPrice => Product.Price * Quantity;

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Cart Cart { get; set; }

    }
}
