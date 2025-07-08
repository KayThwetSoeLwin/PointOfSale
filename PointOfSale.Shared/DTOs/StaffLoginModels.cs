namespace PointOfSale.Shared.DTOs
{
    public class StaffLoginRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } // Add RememberMe property
    }

    public class StaffLoginResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public StaffModel? Data { get; set; }
    }

    public class StaffModel
    {
        public string Username { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
}
