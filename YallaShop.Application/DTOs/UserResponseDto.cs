using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaShop.Application.DTOs
{
    public class UserResponseDto
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        public required string UserName { get; set; }
        public string FullName { get; set; }
        public required string token { get; set; }
		public string Role { get; set; }

		public DateTime TokenExpiryTime { get; set; }
    }
}
