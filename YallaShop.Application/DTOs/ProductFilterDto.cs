namespace YallaShop.Application.DTOs
{
    public class ProductFilterDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? StockQuantity { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public string? SortBy { get; set; } // "price", "name", "date"
        public string? SortOrder { get; set; } = "asc"; // "asc" or "desc"
    }
}
