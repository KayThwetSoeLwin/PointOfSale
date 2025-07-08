namespace PointOfSale.Shared.DTOs
{
    public class SaleCreateRequest
    {
        public string VoucherCode { get; set; } = null!;
        public DateTime? SaleDate { get; set; }
        public List<SaleItemDto> Items { get; set; } = new();
    }

    public class SaleItemDto
    {
        public string ProductCode { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
