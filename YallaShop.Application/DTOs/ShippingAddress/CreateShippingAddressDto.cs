using System.ComponentModel.DataAnnotations;

namespace YallaShop.Application.DTOs.ShippingAddress
{
    public class CreateShippingAddressDto
    {
        [Required] public string Label { get; set; } = string.Empty;
        [Required] public string Street { get; set; } = string.Empty;
        [Required] public string City { get; set; } = string.Empty;
        [Required] public string State { get; set; } = string.Empty;
        [Required] public string Country { get; set; } = string.Empty;
        [Required] public string ZipCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
