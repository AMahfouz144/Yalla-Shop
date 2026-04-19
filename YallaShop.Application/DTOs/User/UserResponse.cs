using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Application.DTOs.User
{
    public class UserResponse
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }


        public bool IsDeleted { get; set; }
    }
}
