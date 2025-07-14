namespace PointOfSale.Database.AppDbContextModels
{
    public class MenuPermission
    {
        public int MenuPermissionId { get; set; }

        public int RoleId { get; set; }
        public int MenuId { get; set; }

        public bool CanAccess { get; set; }

        // Navigation
        public Role Role { get; set; }
        public Menu Menu { get; set; }
    }
}
