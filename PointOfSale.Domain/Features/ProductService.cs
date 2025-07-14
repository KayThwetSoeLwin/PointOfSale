using Microsoft.EntityFrameworkCore;
using PointOfSale.Database.AppDbContextModels;
using PointOfSale.Interfaces;
using PointOfSale.Shared.DTOs;
using PointOfSale.Shared.ViewModels;

namespace PointOfSale.Domain.Features
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            return await _db.Products
                .AsNoTracking()
                .Where(p => p.InActive == true)
                .Select(p => new ProductDto
                {
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity ?? 0
                })
                .ToListAsync();
        }

        public async Task<ProductDto?> FindProductAsync(string code)
        {
            var product = await _db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductCode == code && p.InActive == true);

            return product == null ? null : new ProductDto
            {
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity ?? 0
            };
        }

        public async Task<ProductDto?> FindAnyProductAsync(string code)
        {
            var product = await _db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductCode == code);

            return product == null ? null : new ProductDto
            {
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity ?? 0
            };
        }

        public async Task<ProductCreateResponseModel> CreateProductAsync(ProductCreateRequestModel request)
        {
            var exists = await _db.Products.AnyAsync(p => p.ProductCode == request.ProductCode);
            if (exists)
            {
                return new ProductCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Product code already exists.",
                    CreatedCode = null
                };
            }

            var product = new Product
            {
                ProductCode = request.ProductCode,
                ProductName = request.ProductName,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                InActive = true,
                CreatedAt = DateTime.Now
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return new ProductCreateResponseModel
            {
                IsSuccess = true,
                Message = "Product created successfully.",
                CreatedCode = product.ProductCode
            };
        }

        public async Task<ProductUpdateResponseModel> UpdateProductAsync(ProductUpdateRequestModel request)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p =>
                p.ProductCode == request.ProductCode && p.InActive == true);

            if (product == null)
            {
                return new ProductUpdateResponseModel
                {
                    IsSuccess = false,
                    Message = "Product not found or inactive."
                };
            }

            product.ProductName = request.ProductName;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.ModifiedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ProductUpdateResponseModel
            {
                IsSuccess = true,
                Message = "Product updated successfully."
            };
        }

        public async Task<int> DeleteProductAsync(string code)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductCode == code && p.InActive == true);
            if (product == null) return -1;

            product.InActive = false;
            product.ModifiedAt = DateTime.Now;
            return await _db.SaveChangesAsync();
        }

        public async Task<PagedResult<ProductDto>> GetPaginatedProductsAsync(int pageNumber, int pageSize)
        {
            var query = _db.Products
                .AsNoTracking()
                .Where(p => p.InActive == true)
                .OrderBy(p => p.ProductName);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity ?? 0
                })
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };
        }

    }
}
