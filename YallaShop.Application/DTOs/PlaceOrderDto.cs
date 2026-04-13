using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Enums;

namespace YallaShop.Application.DTOs
{
    public class PlaceOrderDto
    {
        public int CartId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
