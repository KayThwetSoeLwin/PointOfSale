namespace PointOfSale.Shared.DTOs
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpireAt { get; set; }
        public string? Message { get; set; }
    }
}
