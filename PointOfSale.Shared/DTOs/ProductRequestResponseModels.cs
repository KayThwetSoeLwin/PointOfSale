// File: ProductRequestResponseModels.cs
namespace PointOfSale.Shared.DTOs
{
    public class ProductCreateRequestModel
    {
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class ProductCreateResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? CreatedCode { get; set; }
    }

    public class ProductUpdateRequestModel
    {
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class ProductUpdateResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ProductModel
    {
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
