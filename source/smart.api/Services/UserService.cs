using Microsoft.EntityFrameworkCore;
using smart.api.Models;
using smart.contract;
using smart.database;
using smart.resources;
using BCryptNet = BCrypt.Net.BCrypt;

namespace smart.api.Services;

public sealed class UserService : IUserService
{
    #region fields
    private readonly SmartContext _context;
    private readonly IJwtService _jwtUtils;
    private readonly IPasswordRuleService _validator;
    #endregion

    #region ctor
    public UserService(
        SmartContext context,
        IJwtService jwtUtils,
        IPasswordRuleService validator)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _validator = validator;
    }
    #endregion

    #region IUserService
    public async Task<(AuthResponseDto, int)> Authenticate(AuthRequestDto model, string origin)
    {
        var user = await _context
            .Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Username == model.Username);

        // validate
        if (user is null
            || user.PasswordHash is null
            || user.Locked
            || !BCryptNet.Verify(model.Password, user.PasswordHash))
        {
            throw new AppException(SmartResources.Api_Ex_invalid_login);
        }

        // authentication successful so generate jwt and refresh tokens
        var (Token, Expiration) = _jwtUtils.GenerateToken(user);
        var refreshToken = _jwtUtils.GenerateRefreshToken(origin);
        user.RefreshTokens?.Add(refreshToken);


        // remove old refresh tokens from user
        RemoveOldRefreshTokens(user);

        // save changes to db
        _context.Update(user);
        await _context.SaveChangesAsync();

        return (new AuthResponseDto(
            user.Username,
            Token,
            refreshToken.Token,
            Expiration),
            user.Id);
    }
    public async Task<User> GetById(int id)
    {
        var user = await _context
            .Users
            .FirstOrDefaultAsync(u => u.Id == id);
        return user ?? throw new AppException(SmartResources.Api_Ex_user_not_found);
    }
    public async Task<AuthResponseDto> RefreshUserToken(string token, string origin)
    {
        #region get user
        var user = await GetUserByRefreshToken(token);
        if (user is null || user.Locked)
        {
            throw new AppException(SmartResources.Api_Ex_invalid_login);
        }
        #endregion

        #region get refresh token from user
        var refreshToken = user.RefreshTokens?.Single(x => x.Token == token);
        if (refreshToken is null)
        {
            throw new AppException(SmartResources.Api_Ex_refresh_token_not_found);
        }
        #endregion

        #region handle refresh token
        if (refreshToken.IsRevoked)
        {
            // revoke all descendant tokens in case this token has been compromised
            RevokeDescendantRefreshTokens(refreshToken, user, origin, $"Attempted reuse of revoked ancestor token: {token}");
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
        if (!refreshToken.IsActive)
        {
            throw new AppException(SmartResources.Api_Ex_invalid_token);
        }

        // replace old refresh token with a new one (rotate token)
        var newRefreshToken = RotateRefreshToken(refreshToken, origin);

        // remove old refresh tokens from user
        if (user is null || user.RefreshTokens is null)
        {
            throw new AppException(SmartResources.Api_Ex_user_not_found);
        }

        user.RefreshTokens.Add(newRefreshToken);
        RemoveOldRefreshTokens(user);

        // save changes to db
        _context.Update(user);
        _context.SaveChanges();
        #endregion

        #region generate new jwt
        var (Token, Expiration) = _jwtUtils.GenerateToken(user);

        return new AuthResponseDto(
            user.Username,
            Token,
            newRefreshToken.Token,
            Expiration);
        #endregion
    }
    public async Task RevokeToken(string token, string origin)
    {
        var user = await GetUserByRefreshToken(token);
        var refreshToken = user?.RefreshTokens?.Single(x => x.Token == token) ?? RefreshToken.Invalid;

        if (!refreshToken.IsActive)
        {
            throw new AppException(SmartResources.Api_Ex_invalid_token);
        }

        // revoke token and save
        RevokeRefreshToken(refreshToken, origin, "Revoked without replacement");
        if (user is null)
        {
            throw new AppException(SmartResources.Api_Ex_user_not_found);
        }
        _context.Update(user);
        _context.SaveChanges();
    }
    public async Task SetUserPassword(int userId, ChangePasswordRequest model)
    {
        if (model.New != model.Repeated)
        {
            throw new AppException(SmartResources.Api_Ex_repeated_password_different);
        }
        if (!_validator.IsValidPassword(model.New))
        {
            throw new AppException(SmartResources.Api_Ex_invalid_password);
        }
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            throw new AppException(SmartResources.Api_Ex_user_not_found);
        }
        if (user.PasswordHash is null)
        {
            throw new AppException(SmartResources.Api_Ex_invalid_login);
        }


        user.PasswordHash = BCryptNet.HashPassword(model.New);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region helper
    private static void RemoveOldRefreshTokens(User user)
    {
        // remove old inactive refresh tokens from user based on TTL in app settings
        user.RefreshTokens?.RemoveAll(x => !x.IsActive
            && x.Created + TimeSpan.FromDays(7) <= DateTime.UtcNow);
    }
    private async Task<User> GetUserByRefreshToken(string token)
    {
        var user = await _context
            .Users
            .SingleOrDefaultAsync(u => u.RefreshTokens != null
                && u.RefreshTokens.Any(t => t.Token == token));

        return user ?? throw new AppException(SmartResources.Api_Ex_invalid_token);
    }
    private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string ipAddress, string reason)
    {
        // recursively traverse the refresh token chain and ensure all descendants are revoked
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            var childToken = user.RefreshTokens?.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken is null)
            {
                return;
            }
            if (childToken.IsActive)
            {
                RevokeRefreshToken(childToken, ipAddress, reason);
            }
            else
            {
                RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
            }
        }
    }
    private static void RevokeRefreshToken(
        RefreshToken token,
        string ipAddress,
        string reason,
        string? replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }
    private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }
    #endregion

}

