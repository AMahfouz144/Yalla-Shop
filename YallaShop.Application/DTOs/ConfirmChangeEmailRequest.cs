using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Application.DTOs
{
     public class ConfirmChangeEmailRequest
     {
         public string UserId { get; set; }
         public string EmailChangeToken { get; set; }
         public string NewEmail { get; set; }
     }
}
