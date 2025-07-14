using Microsoft.EntityFrameworkCore;
using PointOfSale.Database.AppDbContextModels;
using PointOfSale.Interfaces;
using PointOfSale.Shared.DTOs;
using PointOfSale.Shared.ViewModels;

namespace PointOfSale.Domain.Features
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _db;

        public SaleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<SaleDto>> GetAllSalesAsync()
        {
            return await _db.Sales
                .AsNoTracking()
                .Where(s => s.InActive == true)
                .Select(s => new SaleDto
                {
                    Id = s.SaleId,  // 
                    VoucherCode = s.VoucherCode,
                    SaleDate = s.SaleDate ?? DateTime.MinValue,
                    TotalAmount = s.TotalAmount
                })
                .ToListAsync();
        }

        public async Task<SaleDto?> FindSaleAsync(int id)
        {
            var sale = await _db.Sales
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SaleId == id && s.InActive == true);

            return sale == null ? null : new SaleDto
            {
                Id = sale.SaleId,  //  Matches SaleDto
                VoucherCode = sale.VoucherCode,
                SaleDate = sale.SaleDate ?? DateTime.Now,
                TotalAmount = sale.TotalAmount
            };
        }

        public async Task<List<SaleDetailDto>> GetSaleDetailsByVoucherCodeAsync(string voucherCode)
        {
            var details = await _db.SaleDetails
                .Include(d => d.Product)
                .AsNoTracking()
                .Where(d => d.VoucherCode == voucherCode && d.InActive == true)
                .ToListAsync();

            return details.Select(d => new SaleDetailDto
            {
                Id = d.SaleDetailId,  // ✅ Matches SaleDetailDto
                ProductCode = d.ProductCode,
                VoucherCode = d.VoucherCode,
                ProductName = d.Product?.ProductName ?? "",
                Quantity = d.Quantity,
                Price = d.Price,
                Subtotal = d.Quantity * d.Price
            }).ToList();
        }

        public async Task<int> CreateSaleWithDetailsAsync(SaleCreateRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                return 0;

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                decimal totalAmount = 0;
                var saleDetails = new List<SaleDetail>();

                foreach (var item in request.Items)
                {
                    var product = await _db.Products
                        .FirstOrDefaultAsync(p => p.ProductCode == item.ProductCode && p.InActive == true);

                    if (product == null)
                        continue;

                    var price = product.Price;
                    totalAmount += price * item.Quantity;

                    saleDetails.Add(new SaleDetail
                    {
                        ProductCode = item.ProductCode,
                        VoucherCode = request.VoucherCode,
                        Price = price,
                        Quantity = item.Quantity,
                        InActive = true,
                        CreatedAt = DateTime.Now
                    });
                }

                if (!saleDetails.Any()) return 0;

                var sale = new Sale
                {
                    VoucherCode = request.VoucherCode,
                    SaleDate = request.SaleDate ?? DateTime.Now,
                    TotalAmount = totalAmount,
                    InActive = true,
                    CreatedAt = DateTime.Now
                };

                _db.Sales.Add(sale);
                _db.SaleDetails.AddRange(saleDetails);

                var result = await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                return 0;
            }
        }

        public async Task<int> DeleteSaleAsync(int id)
        {
            var sale = await _db.Sales.FirstOrDefaultAsync(s => s.SaleId == id);
            if (sale == null) return -1;

            sale.InActive = false;
            sale.ModifiedAt = DateTime.Now;

            var relatedDetails = await _db.SaleDetails
                .Where(d => d.VoucherCode == sale.VoucherCode)
                .ToListAsync();

            foreach (var detail in relatedDetails)
            {
                detail.InActive = false;
                detail.ModifiedAt = DateTime.Now;
            }

            return await _db.SaveChangesAsync();
        }

        public async Task<PagedResult<SaleDto>> GetPaginatedSalesAsync(int pageNumber, int pageSize)
        {
            var query = _db.Sales
                .AsNoTracking()
                .Where(s => s.InActive == true)
                .OrderByDescending(s => s.SaleDate);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SaleDto
                {
                    Id = s.SaleId,
                    VoucherCode = s.VoucherCode,
                    SaleDate = s.SaleDate ?? DateTime.Now,
                    TotalAmount = s.TotalAmount
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize); //  Add this

            return new PagedResult<SaleDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages // Set TotalPages here
            };
        }

    }
}
