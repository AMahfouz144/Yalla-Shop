using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace YallaShop.Domain.Entites
{
    public class ShippingAddress : BaseEntity
    {
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string Label { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
