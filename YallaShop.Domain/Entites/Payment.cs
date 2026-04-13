using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Enums;

namespace YallaShop.Domain.Entites
{
     public class Payment:BaseEntity
     {
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public PaymentMethod PaymentMethod { get; set; } 
        public bool IsPaid { get; set; }

        public DateTime PaidAt { get; set; }
    }
}
