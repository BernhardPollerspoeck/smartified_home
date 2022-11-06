using smart.core.Models;
using System.Net;
using System.Text.Json;

namespace smart.api.Middlewares;

public sealed class ErrorHandlerMiddleware
{
    #region fields
    private readonly RequestDelegate _next;
    #endregion

    #region ctor
    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    #endregion

    #region delegate
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = error switch
            {
                AppException aex => (int)(aex.StatusCode ?? HttpStatusCode.BadRequest),// custom application error
                KeyNotFoundException => (int)HttpStatusCode.NotFound,// not found error
                _ => (int)HandleInternalError(error),// unhandled error
            };
            var result = JsonSerializer.Serialize(new
            {
                message = error?.Message,
            });
            await response.WriteAsync(result);
        }
    }
    #endregion

    #region helper
    private static HttpStatusCode HandleInternalError(Exception ex)
    {


        return HttpStatusCode.InternalServerError;
    }
    #endregion
}

