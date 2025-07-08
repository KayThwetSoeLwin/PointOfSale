namespace PointOfSale.Shared.DTOs
{
    public class StaffCreateRequestModel
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime HireDate { get; set; }  // Match with StaffDto
        public string Designation { get; set; } = "";
    }

    public class StaffCreateResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = "";
        public int CreatedId { get; set; } = 0;
    }

    public class StaffUpdateRequestModel
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Designation { get; set; } = "";
        public DateTime HireDate { get; set; }  // Match with StaffDto
    }

    public class StaffUpdateResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = "";
    }
}
