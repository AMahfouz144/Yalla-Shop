using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Application.DTOs
{
 public class ProductResponseDto
 {
     public int Id { get; set; }
     public string Name { get; set; }
     public string Description { get; set; }
     public decimal Price { get; set; }
     public int StockQuantity { get; set; }
     public byte[]? Picture { get; set; }
 }
}
