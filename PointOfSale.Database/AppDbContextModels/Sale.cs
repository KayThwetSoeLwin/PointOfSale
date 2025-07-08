using System;
using System.Collections.Generic;

namespace PointOfSale.Database.AppDbContextModels;

public partial class Sale
{
    public int SaleId { get; set; }

    public string VoucherCode { get; set; } = null!;

    public DateTime? SaleDate { get; set; }

    public decimal TotalAmount { get; set; }

    public bool? InActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
