using System;
using System.Collections.Generic;

namespace PointOfSale.Database.AppDbContextModels;

public partial class Product
{
    public string ProductCode { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int? StockQuantity { get; set; }

    public bool? InActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
