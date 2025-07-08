using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PointOfSale.Database.AppDbContextModels;
using PointOfSale.Interfaces;
using PointOfSale.Shared.DTOs;
using PointOfSale.Shared.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PointOfSale.Domain.Features
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public StaffService(AppDbContext db, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<List<StaffDto>> GetAllStaffAsync()
        {
            return await _db.Staff
                .AsNoTracking()
                .Where(s => s.InActive == true)
                .Select(s => new StaffDto
                {
                    StaffId = s.StaffId,
                    Username = s.Username,
                    FullName = s.FullName,
                    Email = s.Email,
                    Designation = s.Designation
                })
                .ToListAsync();
        }

        public async Task<StaffDto?> FindStaffAsync(int id)
        {
            var staff = await _db.Staff
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == id && s.InActive == true);

            return staff == null ? null : new StaffDto
            {
                StaffId = staff.StaffId,
                Username = staff.Username,
                FullName = staff.FullName,
                Email = staff.Email,
                Designation = staff.Designation
            };
        }

        public async Task<StaffCreateResponseModel> CreateStaffAsync(StaffCreateRequestModel request)
        {
            bool exists = await _db.Staff.AnyAsync(s => s.Username == request.Username);
            if (exists)
            {
                return new StaffCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Username already exists.",
                    CreatedId = -1
                };
            }

            var hashedPassword = ComputeSha256Hash(request.Password);

            var staff = new Staff
            {
                Username = request.Username,
                PasswordHash = hashedPassword,
                FullName = request.FullName,
                Email = request.Email,
                HireDate = request.HireDate,
                Designation = request.Designation,
                InActive = true,
                CreatedAt = DateTime.Now
            };

            _db.Staff.Add(staff);
            await _db.SaveChangesAsync();

            return new StaffCreateResponseModel
            {
                IsSuccess = true,
                Message = "Staff created successfully.",
                CreatedId = staff.StaffId
            };
        }

        public async Task<StaffUpdateResponseModel> UpdateStaffAsync(StaffUpdateRequestModel request)
        {
            var staff = await _db.Staff.FirstOrDefaultAsync(s => s.StaffId == request.StaffId && s.InActive == true);
            if (staff == null)
            {
                return new StaffUpdateResponseModel
                {
                    IsSuccess = false,
                    Message = "Staff not found or inactive."
                };
            }

            staff.FullName = request.FullName;
            staff.Email = request.Email;
            staff.Designation = request.Designation;
            staff.ModifiedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new StaffUpdateResponseModel
            {
                IsSuccess = true,
                Message = "Staff updated successfully."
            };
        }

        public async Task<int> DeleteStaffAsync(int id)
        {
            var staff = await _db.Staff.FirstOrDefaultAsync(s => s.StaffId == id);
            if (staff == null) return -1;

            staff.InActive = false;
            staff.ModifiedAt = DateTime.Now;

            return await _db.SaveChangesAsync();
        }

        // ✅ Login for MVC (Cookie)
        public async Task<StaffLoginResponseModel> LoginAsync(StaffLoginRequestModel requestModel)
        {
            var staff = await _db.Staff.FirstOrDefaultAsync(x => x.Username == requestModel.Username);
            if (staff is null)
            {
                return new StaffLoginResponseModel
                {
                    IsSuccess = false,
                    Message = "User doesn't exist."
                };
            }

            string hashedPassword = ComputeSha256Hash(requestModel.Password);
            if (staff.PasswordHash != hashedPassword)
            {
                return new StaffLoginResponseModel
                {
                    IsSuccess = false,
                    Message = "Invalid username or password."
                };
            }

            // Cookie auth only if HTTP Context exists (MVC browser request)
            if (_httpContextAccessor.HttpContext != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, staff.Username),
                    new Claim(ClaimTypes.Email, staff.Email ?? "")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = requestModel.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                };

                await _httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }

            return new StaffLoginResponseModel
            {
                IsSuccess = true,
                Message = "Login successful.",
                Data = new StaffModel
                {
                    Username = staff.Username,
                    FullName = staff.FullName,
                    Email = staff.Email
                }
            };
        }

        // ✅ Login for API (JWT)
        //public async Task<LoginResult> LoginForApiAsync(StaffLoginRequestModel requestModel)
        //{
        //    var staff = await _db.Staff.FirstOrDefaultAsync(x => x.Username == requestModel.Username);
        //    if (staff == null || staff.PasswordHash != ComputeSha256Hash(requestModel.Password))
        //    {
        //        return new LoginResult
        //        {
        //            IsSuccess = false,
        //            Message = "Invalid credentials."
        //        };

        //    }

        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Name, staff.Username),
        //        new Claim(ClaimTypes.Email, staff.Email ?? "")
        //    };

        //    var secretKey = _configuration["JwtSettings:SecretKey"]!;
        //    var issuer = _configuration["JwtSettings:Issuer"]!;
        //    var audience = _configuration["JwtSettings:Audience"]!;
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        //    var token = new JwtSecurityToken(
        //        issuer: issuer,
        //        audience: audience,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(1),
        //        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        //    );

        //    string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        //    return new LoginResult
        //    {
        //        IsSuccess = true,
        //        Token = jwt,
        //        ExpireAt = token.ValidTo
        //    };
        //}

        public async Task<LoginResult> LoginForApiAsync(StaffLoginRequestModel requestModel)
        {
            Console.WriteLine("LoginForApiAsync() called");
            Console.WriteLine($"Input Username: {requestModel.Username}");
            Console.WriteLine($"Input Raw Password: {requestModel.Password}");

            var enteredHash = ComputeSha256Hash(requestModel.Password);
            Console.WriteLine($"Hashed Input Password: {enteredHash}");

            var staff = await _db.Staff.FirstOrDefaultAsync(x => x.Username == requestModel.Username);

            if (staff == null)
            {
                Console.WriteLine("User not found in database.");
                return new LoginResult
                {
                    IsSuccess = false,
                    Message = "Invalid credentials. (User not found)"
                };
            }

            Console.WriteLine($"Found user in DB: {staff.Username}");
            Console.WriteLine($"Stored DB PasswordHash: {staff.PasswordHash}");

            if (staff.PasswordHash != enteredHash)
            {
                Console.WriteLine(" Password hash mismatch!");
                return new LoginResult
                {
                    IsSuccess = false,
                    Message = "Invalid credentials. (Password mismatch)"
                };
            }

            Console.WriteLine("Password matched. Generating JWT...");

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, staff.Username),
        new Claim(ClaimTypes.Email, staff.Email ?? "")
    };

            var secretKey = _configuration["JwtSettings:SecretKey"]!;
            var issuer = _configuration["JwtSettings:Issuer"]!;
            var audience = _configuration["JwtSettings:Audience"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"JWT Generated: {jwt}");

            return new LoginResult
            {
                IsSuccess = true,
                Token = jwt,
                ExpireAt = token.ValidTo
            };
        }


        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        // to implement pagination

        public async Task<PagedResult<StaffDto>> GetPaginatedStaffAsync(int pageNumber, int pageSize)
        {
            var query = _db.Staff
                .AsNoTracking()
                .Where(s => s.InActive == true)
                .OrderBy(s => s.FullName);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StaffDto
                {
                    StaffId = s.StaffId,
                    Username = s.Username,
                    FullName = s.FullName,
                    Email = s.Email,
                    Designation = s.Designation,
                    HireDate = s.HireDate
                })
                .ToListAsync();

            return new PagedResult<StaffDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}
