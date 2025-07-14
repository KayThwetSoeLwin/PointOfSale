namespace PointOfSale.Shared.ViewModels
{
    public class PaginationModel
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public string ActionName { get; set; } = string.Empty;
    }
}
