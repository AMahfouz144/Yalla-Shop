using System;

namespace YallaShop.API.ViewModels
{
     public class WishlistResponseViewModel
     {
             public int Id { get; set; }
             public string UserId { get; set; }
             public int ProductId { get; set; }
             public DateTime CreatedAt { get; set; }

             public ProductViewModel? Product { get; set; }
     }
}
