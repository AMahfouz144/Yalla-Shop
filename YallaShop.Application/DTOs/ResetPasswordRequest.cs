using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Application.DTOs
{
     public class ResetPasswordRequest
     {
         public string UserId { get; set; }
         public string Code { get; set; }
         public string NewPassword { get; set; }
     }
}
