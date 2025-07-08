using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PointOfSale.Interfaces;
using PointOfSale.Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PointOfSale.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public StaffController(IStaffService service, IEmailService emailService, IConfiguration configuration)
        {
            _service = service;
            _emailService = emailService;
            _configuration = configuration;
        }

        // 🔓 Allow anonymous login (no JWT required)
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] StaffLoginRequestModel requestModel)
        {
            var result = await _service.LoginForApiAsync(requestModel);  // ✅ Use correct field name

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var staffList = await _service.GetAllStaffAsync();

            var result = staffList.Select(s => new StaffDto
            {
                StaffId = s.StaffId,
                Username = s.Username,
                FullName = s.FullName,
                Email = s.Email,
                Designation = s.Designation
            }).ToList();

            return Ok(new { message = "Staff list retrieved successfully", data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var staff = await _service.FindStaffAsync(id);
            if (staff == null)
                return NotFound(new { message = "Staff not found" });

            var result = new StaffDto
            {
                StaffId = staff.StaffId,
                Username = staff.Username,
                FullName = staff.FullName,
                Email = staff.Email,
                Designation = staff.Designation
            };

            return Ok(new { message = "Staff found", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffCreateRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input" });

            var result = await _service.CreateStaffAsync(request);

            if (result.IsSuccess)
            {
                string emailBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <h2>Welcome to the POS System, {request.FullName}!</h2>
                    <p>Your staff account has been <strong>successfully created</strong>.</p>
                    <p>You can now log in to the system using your credentials.</p>
                    <p style='margin-top: 30px; color: gray;'>
                        ⚠️ This is a test email. Login link is not included as this system is under API testing.
                    </p>
                    <br>
                    <p>Best regards,<br><strong>POS Admin Team</strong></p>
                </body>
                </html>";

                await _emailService.SendAsync(request.Email, "Welcome to POS System", emailBody);
            }

            return result.IsSuccess
                ? Ok(new { message = result.Message })
                : BadRequest(new { message = result.Message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StaffUpdateRequestModel request)
        {
            request.StaffId = id;

            var result = await _service.UpdateStaffAsync(request);
            return result.IsSuccess
                ? Ok(new { message = result.Message })
                : NotFound(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteStaffAsync(id);
            return result > 0
                ? Ok(new { message = "Staff deleted successfully" })
                : NotFound(new { message = "Staff not found" });
        }

        
    }
}
