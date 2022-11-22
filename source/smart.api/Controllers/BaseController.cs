using Microsoft.AspNetCore.Mvc;
using smart.core.Models;
using smart.database;
using smart.resources;

namespace smart.api.Controllers;

public abstract class BaseController : ControllerBase
{
    #region properties
    protected int UserId => HttpContext
        .Items["UserId"] as int?
        ?? throw new AppException(SmartResources.Api_Ex_user_not_found);

    protected new User User => HttpContext
        .Items["User"] as User
        ?? throw new AppException(SmartResources.Api_Ex_user_not_found);
    #endregion

    #region methods
    protected string GetOrigin()
    {
        // get source ip address for the current request
        return Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"].ToString()
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() ?? string.Empty;
    }
    #endregion
}
