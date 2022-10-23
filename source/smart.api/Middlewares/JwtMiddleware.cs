
using smart.api.Services;

namespace smart.api.Middlewares;

public sealed class JwtMiddleware
{
    #region fields
    private readonly RequestDelegate _next;
    #endregion

    #region ctor
    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    #endregion

    #region delegate
    public async Task Invoke(HttpContext context, IUserService userService, IJwtService jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token is not null)
        {
            var userId = await jwtUtils.ValidateToken(token);
            if (userId.HasValue)
            {
                var user = await userService.GetById(userId.Value);
                // attach user to context on successful jwt validation
                context.Items["User"] = user;
                context.Items["UserId"] = user.Id;
            }
        }

        await _next(context);
    }
    #endregion
}
