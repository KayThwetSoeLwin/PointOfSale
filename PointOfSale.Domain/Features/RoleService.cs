using Microsoft.EntityFrameworkCore;
using PointOfSale.Database;
using PointOfSale.Database.AppDbContextModels;
using PointOfSale.Interfaces;
using PointOfSale.Shared.DTOs;


namespace PointOfSale.Domain.Features
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _db;

        public RoleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            return await _db.Roles
                .Select(r => new RoleDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToListAsync();
        }
    }
}
