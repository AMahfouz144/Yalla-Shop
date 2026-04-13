using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Domain.Enums;

namespace YallaShop.Application.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }
}
