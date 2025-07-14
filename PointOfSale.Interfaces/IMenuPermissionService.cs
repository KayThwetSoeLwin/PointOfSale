using System.Collections.Generic;
using System.Threading.Tasks;
using PointOfSale.Shared.ViewModels;

namespace PointOfSale.Interfaces
{
    public interface IMenuPermissionService
    {
        Task<RolePermissionManageViewModel> GetPermissionsByRoleAsync(int roleId);
        Task SavePermissionsAsync(int roleId, List<MenuPermissionViewModel> permissions);
    }
}
