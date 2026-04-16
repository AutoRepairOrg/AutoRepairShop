namespace AutoRepairShop.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(Guid userId, string username, string role);
        string GenerateRefreshToken();
    }
}
