namespace YallaShop.API.ViewModels
{
     public class ConfirmChangeEmailViewModel
     {
         public string UserId { get; set; }
         public string EmailChangeToken { get; set; }
         public string NewEmail { get; set; }
     }
}
