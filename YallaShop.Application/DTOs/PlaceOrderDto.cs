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

        // Shipping Address Fields
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}
