using System;

namespace YallaShop.API.ViewModels
{
     public class ReviewResponseViewModel
     {
         public int Id { get; set; }
         public string UserId { get; set; }
         public int ProductId { get; set; }
         public int Rating { get; set; }
         public string? Comment { get; set; }
         public DateTime CreatedAt { get; set; }
     }
}
