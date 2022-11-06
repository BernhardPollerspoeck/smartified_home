
using smart.core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using smart.database;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace smart.api.Services;

public sealed class JwtService : IJwtService
{
    #region fields
    private readonly SmartContext _context;
    private readonly IOptions<ApiSettings> _options;
    #endregion

    #region ctor
    public JwtService(
        SmartContext context,
        IOptions<ApiSettings> options)
    {
        _context = context;
        _options = options;
    }
    #endregion

    #region IJwtService
    public (string Token, DateTime Expiration) GenerateToken(User user)
    {
        // generate token that is valid for 15 minutes
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_options.Value.Secret);
        var expiration = DateTime.UtcNow + TimeSpan.FromMinutes(5);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = expiration,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Claims = new Dictionary<string, object>(),
            Issuer = "Smartified Home",
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expiration);
    }
    public async Task<int?> ValidateToken(string token)
    {
        if (token == null)
        {
            return null;
        }

        var tokenUserId = GetTokenUserId(token);
        if (!tokenUserId.HasValue)
        {
            return null;
        }
        var user = await _context
            .Users
            .FirstOrDefaultAsync(u => u.Id == tokenUserId.Value);
        if (user is null)
        {
            return null;
        }
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_options.Value.Secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // return user id from JWT token if validation successful
            return userId;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
    public RefreshToken GenerateRefreshToken(string origin)
    {

        // generate token that is valid for 7 days
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow + TimeSpan.FromDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = origin
        };

        return refreshToken;
    }
    #endregion

    #region helper
    private static int? GetTokenUserId(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token);
        var tokenS = jsonToken as JwtSecurityToken;
        var idString = tokenS?.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        return int.TryParse(idString, out var id) ? id : null;
    }
    #endregion
}


