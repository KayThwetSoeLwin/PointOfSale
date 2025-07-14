namespace PointOfSale.Shared.ViewModels
{
    public class MenuPermissionViewModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public bool CanAccess { get; set; }
    }
}
