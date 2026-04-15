using AutoRepairShop.Application.DTOs.Auth;

namespace AutoRepairShop.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshRequest request);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
