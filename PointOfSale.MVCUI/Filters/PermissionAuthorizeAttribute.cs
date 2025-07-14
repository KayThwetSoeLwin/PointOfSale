using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Database.AppDbContextModels;
using System.Linq;

namespace PointOfSale.MVCUI.Filters
{
    public class PermissionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _endpoint;

        public PermissionAuthorizeAttribute(string endpoint)
        {
            _endpoint = endpoint;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var dbContext = httpContext.RequestServices.GetService(typeof(AppDbContext)) as AppDbContext;

            var username = httpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToActionResult("Login", "Staff", null);
                return;
            }

            var staff = dbContext.Staff.FirstOrDefault(s => s.Username == username);
            if (staff == null)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            var roleId = staff.RoleId;

            var hasAccess = dbContext.MenuPermissions
                .Include(p => p.Menu)
                .Any(p => p.RoleId == roleId &&
                          p.Menu.Endpoint.ToLower() == _endpoint.ToLower() &&
                          p.CanAccess);

            if (!hasAccess)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}
