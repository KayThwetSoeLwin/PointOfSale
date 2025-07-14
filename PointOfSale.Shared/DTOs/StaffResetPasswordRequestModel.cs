namespace PointOfSale.Shared.DTOs
{
    public class StaffResetPasswordRequestModel
    {
        public int StaffId { get; set; }

        public string NewPassword { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;
    }
}
