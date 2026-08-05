using System.IdentityModel.Tokens.Jwt;
using Ecommerce.Api.Core.Security;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Api.Core.Tests;

public class JwtTokenServiceTests
{
    private static IJwtTokenService CreateService()
    {
        var key = new string('a', 40);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection([new KeyValuePair<string, string?>("JwtSigningKey", key)])
            .Build();
        return new JwtTokenService(config);
    }

    [Fact]
    public void GenerateToken_ReturnsNonNullJwtString()
    {
        var svc = CreateService();
        var token = svc.GenerateToken(1, 42, "user@example.com");
        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public void GenerateToken_ContainsExpectedClaims()
    {
        var svc = CreateService();
        var token = svc.GenerateToken(1, 42, "user@example.com");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("1", jwt.Claims.First(c => c.Type == "sub").Value);
        Assert.Equal("42", jwt.Claims.First(c => c.Type == "customer_id").Value);
        Assert.Equal("user@example.com", jwt.Claims.First(c => c.Type == "email").Value);
        Assert.NotNull(jwt.Claims.FirstOrDefault(c => c.Type == "exp"));
    }

    [Fact]
    public void ValidateToken_ReturnsTrueForValidToken()
    {
        var svc = CreateService();
        var token = svc.GenerateToken(1, 42, "user@example.com");
        Assert.True(svc.ValidateToken(token));
    }

    [Fact]
    public void ValidateToken_ReturnsFalseForGarbage()
    {
        var svc = CreateService();
        Assert.False(svc.ValidateToken("not-a-real-token"));
    }
}