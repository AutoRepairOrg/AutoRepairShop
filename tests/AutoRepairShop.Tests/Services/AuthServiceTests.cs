using AutoRepairShop.Application.DTOs.Auth;
using AutoRepairShop.Application.Interfaces;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Application.Services;
using AutoRepairShop.Domain.Entities;
using AutoRepairShop.Domain.Interfaces.Repositories;
using Moq;

namespace AutoRepairShop.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IAdminRepository> _adminRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();

    [Fact]
    public async Task LoginAsync_WhenCustomerCredentialsAreValid_ShouldReturnTokensAndPersistRefreshToken()
    {
        var request = new LoginRequest { Username = "maria", Password = "secret" };
        var customer = new Customer(
            "Maria",
            Document.Create("12345678909"),
            "11999999999",
            request.Username,
            "hashed-password"
        );
        _customerRepositoryMock
            .Setup(repository => repository.GetByUserNameAsync(request.Username))
            .ReturnsAsync(customer);
        _adminRepositoryMock
            .Setup(repository => repository.GetByUserNameAsync(request.Username))
            .ReturnsAsync((Admin?)null);
        _passwordHasherMock
            .Setup(hasher => hasher.Verify(request.Password, customer.Password))
            .Returns(true);
        _jwtTokenServiceMock
            .Setup(service =>
                service.GenerateAccessToken(customer.Id, customer.Username, "Customer")
            )
            .Returns("access-token");
        _jwtTokenServiceMock
            .Setup(service => service.GenerateRefreshToken())
            .Returns("refresh-token");
        var sut = CreateSut();

        var result = await sut.LoginAsync(request);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal("Customer", result.Role);
        _refreshTokenRepositoryMock.Verify(
            repository =>
                repository.SaveAsync(
                    It.Is<RefreshToken>(token =>
                        token.UserId == customer.Id
                        && token.Token == "refresh-token"
                        && !token.IsRevoked
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ShouldThrow()
    {
        var request = new LoginRequest { Username = "missing", Password = "secret" };
        _customerRepositoryMock
            .Setup(repository => repository.GetByUserNameAsync(request.Username))
            .ReturnsAsync((Customer?)null);
        _adminRepositoryMock
            .Setup(repository => repository.GetByUserNameAsync(request.Username))
            .ReturnsAsync((Admin?)null);
        var sut = CreateSut();

        var action = () => sut.LoginAsync(request);

        var exception = await Assert.ThrowsAsync<Exception>(action);
        Assert.Equal("Invalid credentials", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ShouldThrow()
    {
        var request = new LoginRequest { Username = "maria", Password = "wrong-password" };
        var customer = new Customer(
            "Maria",
            Document.Create("12345678909"),
            "11999999999",
            request.Username,
            "hashed-password"
        );
        _customerRepositoryMock
            .Setup(repository => repository.GetByUserNameAsync(request.Username))
            .ReturnsAsync(customer);
        _adminRepositoryMock
            .Setup(repository => repository.GetByUserNameAsync(request.Username))
            .ReturnsAsync((Admin?)null);
        _passwordHasherMock
            .Setup(hasher => hasher.Verify(request.Password, customer.Password))
            .Returns(false);
        var sut = CreateSut();

        var action = () => sut.LoginAsync(request);

        var exception = await Assert.ThrowsAsync<Exception>(action);
        Assert.Equal("Invalid credentials", exception.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshTokenIsValid_ShouldRevokeOldTokenAndReturnNewTokens()
    {
        var admin = new Admin("Admin", "Ops", "admin", "hashed-password");
        var storedToken = new RefreshToken(
            admin.Id,
            "old-refresh-token",
            DateTime.UtcNow.AddDays(1)
        );
        var request = new RefreshRequest { RefreshToken = storedToken.Token };
        _refreshTokenRepositoryMock
            .Setup(repository => repository.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync(storedToken);
        _adminRepositoryMock
            .Setup(repository => repository.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);
        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(admin.Id))
            .ReturnsAsync((Customer?)null);
        _jwtTokenServiceMock
            .Setup(service => service.GenerateAccessToken(admin.Id, admin.Username, "Admin"))
            .Returns("new-access-token");
        _jwtTokenServiceMock
            .Setup(service => service.GenerateRefreshToken())
            .Returns("new-refresh-token");
        var sut = CreateSut();

        var result = await sut.RefreshTokenAsync(request);

        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);
        Assert.True(storedToken.IsRevoked);
        _refreshTokenRepositoryMock.Verify(
            repository =>
                repository.SaveAsync(
                    It.Is<RefreshToken>(token =>
                        token.UserId == admin.Id && token.Token == "new-refresh-token"
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshTokenIsInvalid_ShouldThrow()
    {
        var request = new RefreshRequest { RefreshToken = "invalid-token" };
        _refreshTokenRepositoryMock
            .Setup(repository => repository.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync((RefreshToken?)null);
        var sut = CreateSut();

        var action = () => sut.RefreshTokenAsync(request);

        var exception = await Assert.ThrowsAsync<Exception>(action);
        Assert.Equal("Invalid refresh token", exception.Message);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_WhenTokenExists_ShouldMarkTokenAsRevoked()
    {
        var token = new RefreshToken(Guid.NewGuid(), "refresh-token", DateTime.UtcNow.AddDays(1));
        _refreshTokenRepositoryMock
            .Setup(repository => repository.GetByTokenAsync(token.Token))
            .ReturnsAsync(token);
        var sut = CreateSut();

        await sut.RevokeRefreshTokenAsync(token.Token);

        Assert.True(token.IsRevoked);
    }

    private AuthService CreateSut()
    {
        return new AuthService(
            _customerRepositoryMock.Object,
            _adminRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _refreshTokenRepositoryMock.Object
        );
    }
}
