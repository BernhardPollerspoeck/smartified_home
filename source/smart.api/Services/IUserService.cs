using smart.contract;
using smart.database;

namespace smart.api.Services;

public interface IUserService
{
    Task<(AuthResponseDto, int)> Authenticate(AuthRequestDto model, string origin);
    Task<User> GetById(int id);
    Task<AuthResponseDto> RefreshUserToken(string token, string origin);
    Task RevokeToken(string token, string origin);

    Task SetUserPassword(int userId, ChangePasswordRequest model);
}

