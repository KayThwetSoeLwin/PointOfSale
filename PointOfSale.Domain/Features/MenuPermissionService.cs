using Microsoft.EntityFrameworkCore;
using PointOfSale.Database.AppDbContextModels;
using PointOfSale.Interfaces;
using PointOfSale.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Domain.Features
{
    public class MenuPermissionService : IMenuPermissionService
    {
        private readonly AppDbContext _context;

        public MenuPermissionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RolePermissionManageViewModel> GetPermissionsByRoleAsync(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return null;

            var allMenus = await _context.Menus.ToListAsync();
            var existingPermissions = await _context.MenuPermissions
                .Where(p => p.RoleId == roleId)
                .ToListAsync();

            var permissions = allMenus.Select(menu =>
            {
                var access = existingPermissions.FirstOrDefault(p => p.MenuId == menu.MenuId)?.CanAccess ?? false;

                return new MenuPermissionViewModel
                {
                    MenuId = menu.MenuId,
                    MenuName = menu.Name,
                    CanAccess = access
                };
            }).ToList();

            return new RolePermissionManageViewModel
            {
                SelectedRoleId = role.RoleId,
                SelectedRoleName = role.RoleName,
                Permissions = permissions
            };
        }

        public async Task SavePermissionsAsync(int roleId, List<MenuPermissionViewModel> permissions)
        {
            var existingPermissions = await _context.MenuPermissions
                .Where(p => p.RoleId == roleId)
                .ToListAsync();

            foreach (var item in permissions)
            {
                var existing = existingPermissions.FirstOrDefault(p => p.MenuId == item.MenuId);

                if (existing != null)
                {
                    existing.CanAccess = item.CanAccess;
                    _context.MenuPermissions.Update(existing);
                }
                else
                {
                    var newPermission = new MenuPermission
                    {
                        RoleId = roleId,
                        MenuId = item.MenuId,
                        CanAccess = item.CanAccess
                    };
                    await _context.MenuPermissions.AddAsync(newPermission);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
