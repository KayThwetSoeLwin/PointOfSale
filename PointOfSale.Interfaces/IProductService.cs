using PointOfSale.Shared.DTOs;
using PointOfSale.Shared.ViewModels;

namespace PointOfSale.Interfaces
{
    public interface IProductService
    {
        Task<ProductCreateResponseModel> CreateProductAsync(ProductCreateRequestModel request);
        Task<int> DeleteProductAsync(string code);
        Task<ProductDto?> FindAnyProductAsync(string code);
        Task<ProductDto?> FindProductAsync(string code);
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<ProductUpdateResponseModel> UpdateProductAsync(ProductUpdateRequestModel request);
        Task<PagedResult<ProductDto>> GetPaginatedProductsAsync(int pageNumber, int pageSize);
    }
}
