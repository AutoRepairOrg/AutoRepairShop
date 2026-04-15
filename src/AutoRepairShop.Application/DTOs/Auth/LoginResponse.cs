namespace AutoRepairShop.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public DateTime ExpiresAt { get; set; }
        public string Role { get; set; }
    }
}
