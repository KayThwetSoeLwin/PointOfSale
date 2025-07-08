namespace PointOfSale.Shared.DTOs
{
    public class StaffDto
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public DateTime? HireDate { get; set; }  // Add this line
    }
}
