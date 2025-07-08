using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSale.Database.AppDbContextModels;

public partial class SaleDetail
{
    public int SaleDetailId { get; set; }

    public string? ProductCode { get; set; }

    public string? VoucherCode { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public bool? InActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    [ForeignKey("ProductCode")]
    public Product Product { get; set; } = null!;
}
