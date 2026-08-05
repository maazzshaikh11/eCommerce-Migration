namespace Ecommerce.Api.Core.Security;

public interface IJwtTokenService
{
    string GenerateToken(int userId, int customerId, string email);
    bool ValidateToken(string token);
}