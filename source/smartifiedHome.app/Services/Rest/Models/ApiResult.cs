using System.Net;

namespace smartifiedHome.app.Services.Rest.Models;
public class ApiResult
{
    public bool Success { get; init; }

    public HttpStatusCode StatusCode { get; set; }
}

public class ApiResult<TResult> : ApiResult
    where TResult : class?
{
    public TResult? Data { get; init; }

}

