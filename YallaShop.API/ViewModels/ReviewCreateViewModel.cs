namespace YallaShop.API.ViewModels
{
     public class ReviewCreateViewModel
     {
         public string UserId { get; set; }
         public int ProductId { get; set; }
         public int Rating { get; set; }
         public string? Comment { get; set; }
     }
}
