using bp.net.Auth.Server.Attributes;
using Microsoft.AspNetCore.Mvc;
using smart.api.Attributes;
using smart.api.Services;
using smart.contract;
using smart.resources;

namespace smart.api.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public sealed class AuthController : BaseController
{
    #region fields
    private readonly IUserService _userService;
    #endregion

    #region ctor
    public AuthController(
        IUserService userService)
    {
        _userService = userService;
    }
    #endregion

    #region calls
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(AuthRequestDto model)
    {//authenticates the user
        var (response, _) = await _userService.Authenticate(model, GetOrigin());
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest model)
    {//refreshes the user refresh token and session
        var response = await _userService.RefreshUserToken(model.Token ?? string.Empty, GetOrigin());
        return Ok(response);
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken(RevokeTokenRequest model)
    {//revokes the token from the user
        await _userService.RevokeToken(model.Token, GetOrigin());
        return Ok(new MessageResponseDto(SmartResources.Api_Msg_token_revoked));
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest model)
    {//sets your own password
        await _userService.SetUserPassword(UserId, model);
        return Ok();
    }
    #endregion
}
