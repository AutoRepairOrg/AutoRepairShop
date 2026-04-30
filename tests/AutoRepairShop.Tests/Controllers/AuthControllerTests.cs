using AutoRepairShop.Api.Controllers;
using AutoRepairShop.Application.DTOs.Auth;
using AutoRepairShop.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoRepairShop.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();

    [Fact]
    public async Task Login_WhenServiceReturnsToken_ShouldReturnOkWithPayload()
    {
        var request = new LoginRequest { Username = "admin", Password = "secret" };
        var response = new LoginResponse
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Role = "Admin"
        };
        _authServiceMock.Setup(service => service.LoginAsync(request)).ReturnsAsync(response);
        var controller = new AuthController(_authServiceMock.Object);

        var result = await controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
        _authServiceMock.Verify(service => service.LoginAsync(request), Times.Once);
    }

    [Fact]
    public async Task Refresh_WhenServiceReturnsToken_ShouldReturnOkWithPayload()
    {
        var request = new RefreshRequest { RefreshToken = "refresh-token" };
        var response = new LoginResponse
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Role = "Customer"
        };
        _authServiceMock.Setup(service => service.RefreshTokenAsync(request)).ReturnsAsync(response);
        var controller = new AuthController(_authServiceMock.Object);

        var result = await controller.Refresh(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, okResult.Value);
        _authServiceMock.Verify(service => service.RefreshTokenAsync(request), Times.Once);
    }

    [Fact]
    public async Task Logout_WhenRequestIsValid_ShouldReturnNoContent()
    {
        var request = new RefreshRequest { RefreshToken = "refresh-token" };
        var controller = new AuthController(_authServiceMock.Object);

        var result = await controller.Logout(request);

        Assert.IsType<NoContentResult>(result);
        _authServiceMock.Verify(service => service.RevokeRefreshTokenAsync(request.RefreshToken), Times.Once);
    }
}