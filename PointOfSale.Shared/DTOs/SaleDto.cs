namespace PointOfSale.Shared.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public string VoucherCode { get; set; } = null!;
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
