using AutoRepairShop.Application.DTOs.Auth;
using AutoRepairShop.Application.Interfaces;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces;
using AutoRepairShop.Domain.Interfaces.Repositories;

namespace AutoRepairShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            ICustomerRepository customerRepository,
            IAdminRepository adminRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _customerRepository = customerRepository;
            _adminRepository = adminRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            IUser? user;

            var customer = await _customerRepository.GetByUserNameAsync(request.Username);
            var admin = await _adminRepository.GetByUserNameAsync(request.Username);

            user = customer != null ? customer : admin;

            if (user == null)
                throw new Exception("Invalid credentials");

            if (!_passwordHasher.Verify(request.Password, user.Password))
                throw new Exception("Invalid credentials");

            var role = user is Admin ? "Admin" : "Customer";

            var accessToken = _jwtService.GenerateAccessToken(
                user.Id,
                user.Username,
                role
            );

            var refreshToken = _jwtService.GenerateRefreshToken();

            await _refreshTokenRepository.SaveAsync(new RefreshToken(
                user.Id,
                refreshToken,
                DateTime.UtcNow.AddDays(7)
            ));

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshRequest request)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (storedToken == null || !storedToken.IsValid)
                throw new Exception("Invalid refresh token");

            IUser? user;
            var admin = await _adminRepository.GetByIdAsync(storedToken.UserId); ;
            var customer = await _customerRepository.GetByIdAsync(storedToken.UserId);

            user = admin is null ? customer : admin;

            if (user == null)
                throw new Exception("User not found");

            var role = user is Admin ? "Admin" : "Customer";

            var newAccessToken = _jwtService.GenerateAccessToken(
                user.Id,
                user.Username,
                role
            );

            var newRefreshToken = _jwtService.GenerateRefreshToken();

            storedToken.Revoke();

            await _refreshTokenRepository.SaveAsync(new RefreshToken(
                user.Id,
                newRefreshToken,
                DateTime.UtcNow.AddDays(7)
            ));

            return new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token != null)
            {
                token.Revoke();
            }
        }
    }
}