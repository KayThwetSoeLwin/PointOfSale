using PointOfSale.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PointOfSale.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync(); // ✅ Return DTOs instead of SelectListItem
    }
}
