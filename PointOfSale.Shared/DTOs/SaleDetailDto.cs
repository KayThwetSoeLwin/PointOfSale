namespace PointOfSale.Shared.DTOs
{
    public class SaleDetailDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = null!;
        public string VoucherCode { get; set; } = null!;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
