using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using PointOfSale.Interfaces;
using PointOfSale.MVCUI.Filters;
using PointOfSale.MVCUI.ViewModels;
using PointOfSale.Shared.DTOs;
using PointOfSale.Shared.ViewModels;
using System.Security.Claims;

namespace PointOfSale.MVCUI.Controllers;

[Authorize]
public class StaffController : Controller
{
    private readonly IStaffService _service;
    private readonly IEmailService _emailService;
    private readonly IRoleService _roleService;
    private readonly PaginationConfig _paginationConfig;

    public StaffController(
        IStaffService service,
        IEmailService emailService,
        IRoleService roleService,
        IOptions<PaginationConfig> paginationOptions)
    {
        _service = service;
        _emailService = emailService;
        _roleService = roleService;
        _paginationConfig = paginationOptions.Value;
    }

    [AllowAnonymous]
    public IActionResult Login() => View();

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(StaffLoginRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Invalid input.");
            return View(model);
        }

        var result = await _service.LoginAsync(model);
        if (result.IsSuccess)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.Data.Username),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", result.Message);
        return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [PermissionAuthorize("/Staff/Create")]
    public async Task<IActionResult> Create()
    {
        var model = new StaffCreateViewModel
        {
            Roles = await GetRoleDropdownAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("/Staff/Create")]
    public async Task<IActionResult> Create(StaffCreateViewModel staff)
    {
        if (!ModelState.IsValid)
        {
            staff.Roles = await GetRoleDropdownAsync();
            ModelState.AddModelError("", "Invalid input.");
            return View(staff);
        }

        var result = await _service.CreateStaffAsync(staff);
        if (result.IsSuccess)
        {
            string emailBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <h2>Welcome to the POS System, {staff.FullName}!</h2>
                    <p>Your staff account has been <strong>successfully created</strong>.</p>
                    <p>You can now log in to the system using your credentials.</p>
                    <p style='margin-top: 30px; color: gray;'>
                        ⚠️ This is a test email. Login link is not included as this system is under local development.
                    </p>
                    <br>
                    <p>Best regards,<br><strong>POS Admin Team</strong></p>
                </body>
                </html>";

            await _emailService.SendAsync(staff.Email, "Welcome to the POS System", emailBody);
            return RedirectToAction(nameof(Index));
        }

        staff.Roles = await GetRoleDropdownAsync();
        ModelState.AddModelError("", result.Message);
        return View(staff);
    }

    [PermissionAuthorize("/Staff/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var staff = await _service.FindStaffAsync(id);
        if (staff == null) return NotFound();

        var model = new StaffUpdateViewModel
        {
            StaffId = staff.StaffId,
            FullName = staff.FullName,
            Email = staff.Email,
            Designation = staff.Designation,
            HireDate = staff.HireDate ?? DateTime.Today,
            RoleId = staff.RoleId,
            Roles = await GetRoleDropdownAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("/Staff/Edit")]
    public async Task<IActionResult> Edit(StaffUpdateViewModel staff)
    {
        if (!ModelState.IsValid)
        {
            staff.Roles = await GetRoleDropdownAsync();
            ModelState.AddModelError("", "Invalid input.");
            return View(staff);
        }

        var result = await _service.UpdateStaffAsync(staff);
        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        staff.Roles = await GetRoleDropdownAsync();
        ModelState.AddModelError("", result.Message);
        return View(staff);
    }

    [PermissionAuthorize("/Staff/Details")]
    public async Task<IActionResult> Details(int id)
    {
        var staff = await _service.FindStaffAsync(id);
        if (staff == null) return NotFound();

        var model = new StaffDto
        {
            StaffId = staff.StaffId,
            FullName = staff.FullName,
            Username = staff.Username,
            Email = staff.Email,
            Designation = staff.Designation
        };

        return View(model);
    }

    [PermissionAuthorize("/Staff/Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var staff = await _service.FindStaffAsync(id);
        if (staff == null) return NotFound();

        var model = new StaffDto
        {
            StaffId = staff.StaffId,
            FullName = staff.FullName,
            Username = staff.Username,
            Email = staff.Email,
            Designation = staff.Designation
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("/Staff/Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteStaffAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [PermissionAuthorize("/Staff/Index")]
    public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
    {
        int currentPage = pageNumber ?? _paginationConfig.DefaultPageNumber;
        int currentSize = pageSize ?? _paginationConfig.DefaultPageSize;

        var result = await _service.GetPaginatedStaffAsync(currentPage, currentSize);

        if (TempData["Success"] != null)
            ViewBag.Success = TempData["Success"];

        return View(result);
    }

    private async Task<List<SelectListItem>> GetRoleDropdownAsync()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return roles.Select(r => new SelectListItem
        {
            Value = r.RoleId.ToString(),
            Text = r.RoleName
        }).ToList();
    }

    [PermissionAuthorize("/Staff/ResetPassword")]
    public async Task<IActionResult> ResetPassword(int id)
    {
        var staff = await _service.FindStaffAsync(id);
        if (staff == null) return NotFound();

        var model = new StaffResetPasswordViewModel
        {
            StaffId = staff.StaffId,
            Username = staff.Username ?? string.Empty,
            FullName = staff.FullName ?? string.Empty,
            NewPassword = string.Empty,
            ConfirmPassword = string.Empty
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize("/Staff/ResetPassword")]
    public async Task<IActionResult> ResetPassword(StaffResetPasswordViewModel model)
    {
        // Always reload staff info to display in case of error
        var staff = await _service.FindStaffAsync(model.StaffId);
        if (staff == null)
        {
            ModelState.AddModelError("", "Staff not found.");
            return View(model);
        }

        model.FullName = staff.FullName ?? string.Empty;
        model.Username = staff.Username ?? string.Empty;

        if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 6)
        {
            ModelState.AddModelError("", "Password must be at least 6 characters.");
            return View(model);
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError("", "Passwords do not match.");
            return View(model);
        }

        var result = await _service.ResetPasswordByAdminAsync(model.StaffId, model.NewPassword);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

}
