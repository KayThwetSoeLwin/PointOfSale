namespace PointOfSale.Shared.DTOs
{
    public class ProductDto
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
