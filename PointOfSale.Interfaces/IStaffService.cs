using PointOfSale.Shared.DTOs;
using PointOfSale.Shared.ViewModels;

namespace PointOfSale.Interfaces
{
    public interface IStaffService
    {
        Task<StaffCreateResponseModel> CreateStaffAsync(StaffCreateRequestModel request);
        Task<int> DeleteStaffAsync(int id);
        Task<StaffDto?> FindStaffAsync(int id);
        Task<List<StaffDto>> GetAllStaffAsync();
        Task<StaffLoginResponseModel> LoginAsync(StaffLoginRequestModel requestModel);
        Task<StaffUpdateResponseModel> UpdateStaffAsync(StaffUpdateRequestModel request);
        //  NEW: Paginated staff list
        Task<PagedResult<StaffDto>> GetPaginatedStaffAsync(int pageNumber, int pageSize);
        Task<LoginResult> LoginForApiAsync(StaffLoginRequestModel requestModel);
        Task<BaseResponse> ResetPasswordAsync(StaffResetPasswordRequestModel request);
        Task<BaseResponse> ResetPasswordByAdminAsync(int staffId, string newPassword);

    }
}
