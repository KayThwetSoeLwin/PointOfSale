using System;
using System.Collections.Generic;

namespace PointOfSale.Database.AppDbContextModels;

public partial class Staff
{
    public int StaffId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public DateTime? HireDate { get; set; }

    public string? Designation { get; set; }

    public bool? InActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
    public int RoleId { get; set; }  // New field for role-based access
    public Role Role { get; set; }
}
