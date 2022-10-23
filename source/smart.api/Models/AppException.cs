using System.Globalization;
using System.Net;

namespace smart.api.Models;
public sealed class AppException : Exception
{
    public HttpStatusCode? StatusCode { get; }

    public AppException() : base() { }

    public AppException(string message) : base(message) { }

    public AppException(string message, HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public AppException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }

}

