namespace PointOfSale.MVCUI.Controllers;

[Authorize]
public class StaffController : Controller
{
    private readonly IStaffService _service;
    private readonly IEmailService _emailService;

    public StaffController(IStaffService service, IEmailService emailService)
    {
        _service = service;
        _emailService = emailService;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

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
                //new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", result.Message);
        return View(model);
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Staff");
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StaffCreateRequestModel staff)
    {
        Console.WriteLine("Create POST triggered");

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Invalid input.");
            return View(staff);
        }

        var result = await _service.CreateStaffAsync(staff);
        Console.WriteLine("Save result = " + result.IsSuccess);

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

        ModelState.AddModelError("", result.Message);
        return View(staff);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var staff = await _service.FindStaffAsync(id);
        if (staff == null) return NotFound();

        var model = new StaffUpdateRequestModel
        {
            StaffId = staff.StaffId,
            FullName = staff.FullName,
            Email = staff.Email,
            Designation = staff.Designation,
            HireDate = staff.HireDate ?? DateTime.Now
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StaffUpdateRequestModel staff)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Invalid input.");
            return View(staff);
        }

        var result = await _service.UpdateStaffAsync(staff);

        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError("", result.Message);
        return View(staff);
    }

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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteStaffAsync(id);
        return RedirectToAction(nameof(Index));
    }

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

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
    {
        var pagedStaff = await _service.GetPaginatedStaffAsync(pageNumber, pageSize);
        return View(pagedStaff);
    }
}