using PointOfSale.Shared.DTOs;
using PointOfSale.Shared.ViewModels;

namespace PointOfSale.Interfaces
{
    public interface ISaleService
    {
        Task<int> CreateSaleWithDetailsAsync(SaleCreateRequest request);
        Task<int> DeleteSaleAsync(int id);
        Task<SaleDto?> FindSaleAsync(int id);
        Task<List<SaleDto>> GetAllSalesAsync();
        Task<List<SaleDetailDto>> GetSaleDetailsByVoucherCodeAsync(string voucherCode);
        Task<PagedResult<SaleDto>> GetPaginatedSalesAsync(int pageNumber, int pageSize);

    }
}
