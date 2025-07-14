using System.Collections.Generic;

namespace PointOfSale.Shared.ViewModels
{
    public class RolePermissionManageViewModel
    {
        public int SelectedRoleId { get; set; }
        public string SelectedRoleName { get; set; }

        public List<MenuPermissionViewModel> Permissions { get; set; }
    }
}
