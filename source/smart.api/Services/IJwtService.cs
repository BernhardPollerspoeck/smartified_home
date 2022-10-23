using smart.database;

namespace smart.api.Services;

public interface IJwtService
{
    (string Token, DateTime Expiration) GenerateToken(User user);
    Task<int?> ValidateToken(string token);
    RefreshToken GenerateRefreshToken(string origin);
}


