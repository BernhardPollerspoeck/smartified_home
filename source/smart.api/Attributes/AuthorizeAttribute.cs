using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using smart.api.Attributes;
using smart.database;

namespace bp.net.Auth.Server.Attributes;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    #region IAuthorizationFilter
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        #region skip authorization if action is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context
            .ActionDescriptor
            .EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .Any();
        if (allowAnonymous)
        {
            return;
        }
        #endregion

        #region authorization
        var user = (User?)context.HttpContext.Items["User"];
        if (user is null || user.Locked)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }
        #endregion
    }
    #endregion
}
