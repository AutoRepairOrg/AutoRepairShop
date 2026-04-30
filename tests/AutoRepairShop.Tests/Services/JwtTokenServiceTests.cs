using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoRepairShop.Application.Services;
using Microsoft.Extensions.Configuration;

namespace AutoRepairShop.Tests.Services;

public class JwtTokenServiceTests
{
    [Fact]
    public void GenerateAccessToken_WhenConfigurationIsValid_ShouldEmitTokenWithExpectedClaims()
    {
        var configuration = BuildConfiguration();
        var sut = new JwtTokenService(configuration);
        var userId = Guid.NewGuid();

        var token = sut.GenerateAccessToken(userId, "maria", "Customer");

        var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("issuer", parsedToken.Issuer);
        Assert.Equal("audience", parsedToken.Audiences.Single());
        Assert.Equal(
            userId.ToString(),
            parsedToken.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier).Value
        );
        Assert.Equal(
            "maria",
            parsedToken.Claims.Single(claim => claim.Type == ClaimTypes.Name).Value
        );
        Assert.Equal(
            "Customer",
            parsedToken.Claims.Single(claim => claim.Type == ClaimTypes.Role).Value
        );
    }

    [Fact]
    public void GenerateRefreshToken_WhenCalledTwice_ShouldReturnDistinctNonEmptyTokens()
    {
        var sut = new JwtTokenService(BuildConfiguration());

        var first = sut.GenerateRefreshToken();
        var second = sut.GenerateRefreshToken();

        Assert.False(string.IsNullOrWhiteSpace(first));
        Assert.False(string.IsNullOrWhiteSpace(second));
        Assert.NotEqual(first, second);
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "0123456789ABCDEF0123456789ABCDEF",
                    ["Jwt:Issuer"] = "issuer",
                    ["Jwt:Audience"] = "audience",
                    ["Jwt:ExpiresInMinutes"] = "60",
                }
            )
            .Build();
    }
}
