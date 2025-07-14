using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Database.AppDbContextModels;
using PointOfSale.Interfaces;
using PointOfSale.MVCUI.Filters;
using PointOfSale.Shared.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.MVCUI.Controllers
{
    [Authorize]
    public class MenuPermissionController : Controller
    {
        private readonly IMenuPermissionService _menuPermissionService;
        private readonly AppDbContext _context;

        public MenuPermissionController(IMenuPermissionService menuPermissionService, AppDbContext context)
        {
            _menuPermissionService = menuPermissionService;
            _context = context;
        }

        [HttpGet]
        [PermissionAuthorize("/MenuPermission/Manage")]
        public async Task<IActionResult> Manage(int? roleId)
        {
            var roles = await _context.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName");

            if (roleId == null)
                return View(null);

            var model = await _menuPermissionService.GetPermissionsByRoleAsync(roleId.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize("/MenuPermission/Manage")]
        public async Task<IActionResult> Manage(RolePermissionManageViewModel model)
        {
            await _menuPermissionService.SavePermissionsAsync(model.SelectedRoleId, model.Permissions);
            TempData["Success"] = "Permissions updated successfully.";
            return RedirectToAction("Manage", new { roleId = model.SelectedRoleId });
        }
    }
}
