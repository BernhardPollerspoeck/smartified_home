using smart.contract;
using smartifiedHome.app.Services.Rest.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace smartifiedHome.app.Services.Rest;

public class ApiClient : IApiClient
{
    #region fields
    private AuthResponseDto? _session;
    #endregion

    #region properties
    public string? BaseUrl { get; set; }

    protected bool Authenticated => _session is
    {
        JwtToken.Length: > 0,
        RefreshToken.Length: > 0,
    }
    && _session.Expiration > DateTime.UtcNow;
    protected bool CanRefresh => _session is
    {
        JwtToken.Length: > 0,
        RefreshToken.Length: > 0,
    };
    #endregion

    #region IApiClient
    public async Task<InitResult> Init()
    {
        var baseUrl = Preferences.Get(AppShell.SETTING_URL, null);
        if (baseUrl is null)
        {
            return new InitResult
            {
                MissingRoute = true,
            };
        }

        var session = Preferences.Get(AppShell.SETTING_SESSION, null);
        if (session is null)
        {
            return new InitResult
            {
                Expired = true,
            };
        }

        var sessionItem = JsonSerializer.Deserialize<AuthResponseDto>(session);
        _session = sessionItem;
        if (!Authenticated && CanRefresh)
        {
            await RefreshSession();
        }

        return new InitResult
        {
            Expired = !Authenticated,
            Success = Authenticated,
        };
    }
    public async Task<ApiResult<AuthResponseDto>> Authenticate(string username, string password)
    {
        var result = await PostAsync<AuthResponseDto>("Auth/authenticate", new AuthRequestDto(username, password), true, true);
        if (result is { Success: true, Data: not null })
        {
            await SaveSession(result.Data);
            _session = result.Data;
        }
        return result;
    }
    public async Task<ApiResult<AuthResponseDto>> RefreshSession()
    {
        if (!CanRefresh)
        {
            return new ApiResult<AuthResponseDto>();
        }
        var result = await PostAsync<AuthResponseDto>("Auth/refresh-token", new RefreshTokenRequest(_session!.RefreshToken), false, false);
        if (result.Success && result.Data is not null)
        {
            await SaveSession(result.Data);
            _session = result.Data;
        }
        return result;
    }
    public Task Logout()
    {
        _session = null;
        Preferences.Remove(AppShell.SETTING_SESSION);
        return Task.CompletedTask;
    }
    #endregion

    #region call executor
    protected async Task<ApiResult<TResult>> GetAsync<TResult>(string route, bool allowedToRefresh, bool allowedToRecall)
        where TResult : class?
    {
        await HandleSessionRefreshing(allowedToRefresh);
        var client = GetClient();
        if (client is null)
        {
            return new ApiResult<TResult>();
        }
        try
        {
            var result = await client.GetAsync(route);
            if (result.StatusCode == HttpStatusCode.Unauthorized && CanRefresh && allowedToRecall)
            {
                var innerResult = await GetAsync<TResult>(route, true, false);
                return innerResult;
            }

            return new ApiResult<TResult>
            {
                Success = result.IsSuccessStatusCode,
                StatusCode = result.StatusCode,
                Data = result.IsSuccessStatusCode ? await JsonSerializer.DeserializeAsync<TResult>(await result.Content.ReadAsStreamAsync()) : null,
            };
        }
        catch
        {
            return new ApiResult<TResult>();
        }
    }
    protected async Task<ApiResult<TResult>> PostAsync<TResult>(string route, object? body, bool allowedToRefresh, bool allowedToRecall)
        where TResult : class?
    {
        await HandleSessionRefreshing(allowedToRefresh);
        var client = GetClient();
        if (client is null)
        {
            return new ApiResult<TResult>();
        }
        try
        {
            var content = GetContent(body);
            var result = await client.PostAsync(route, content);
            if (result.StatusCode == HttpStatusCode.Unauthorized && CanRefresh && allowedToRecall)
            {
                var innerResult = await PostAsync<TResult>(route, body, true, false);
                return innerResult;
            }

            return new ApiResult<TResult>
            {
                Success = result.IsSuccessStatusCode,
                Data = result.IsSuccessStatusCode ? await JsonSerializer.DeserializeAsync<TResult>(await result.Content.ReadAsStreamAsync()) : null,
                StatusCode = result.StatusCode
            };
        }
        catch
        {
            return new ApiResult<TResult>();
        }
    }
    protected async Task<ApiResult> PostAsync(string route, object? body, bool allowedToRefresh, bool allowedToRecall)
    {
        await HandleSessionRefreshing(allowedToRefresh);

        var client = GetClient();
        if (client is null)
        {
            return new ApiResult();
        }
        try
        {
            var content = GetContent(body);
            var result = await client.PostAsync(route, content);
            if (result.StatusCode == HttpStatusCode.Unauthorized && CanRefresh && allowedToRecall)
            {
                var innerResult = await PostAsync<ApiResult>(route, body!, true, false);
                return innerResult;
            }

            return new ApiResult
            {
                Success = result.IsSuccessStatusCode,
                StatusCode = result.StatusCode,
            };
        }
        catch
        {
            return new ApiResult();
        }
    }
    protected async Task<ApiResult> PutAsync(string route, object? body, bool allowedToRefresh, bool allowedToRecall)
    {
        await HandleSessionRefreshing(allowedToRefresh);
        var client = GetClient();
        if (client is null)
        {
            return new ApiResult();
        }
        try
        {
            var content = GetContent(body);
            var result = await client.PutAsync(route, content);
            if (result.StatusCode == HttpStatusCode.Unauthorized && CanRefresh && allowedToRecall)
            {
                var innerResult = await PutAsync(route, body!, true, false);
                return innerResult;
            }

            return new ApiResult
            {
                Success = result.IsSuccessStatusCode,
                StatusCode = result.StatusCode,
            };
        }
        catch
        {
            return new ApiResult();
        }
    }
    #endregion

    #region helper
    private HttpClient? GetClient()
    {
        if (BaseUrl is null || _session is not { JwtToken: not null })
        {
            return null;
        }
        var client = new HttpClient(
            new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    //TODO: ssl bypass 
                    true,
            }
            , false)
        {
            BaseAddress = new Uri(BaseUrl)
        };
        if (Authenticated)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_session.JwtToken}");
        }
        return client;
    }
    private Task HandleSessionRefreshing(bool allowedToRefresh)
    {
        return !Authenticated && CanRefresh && allowedToRefresh
            ? RefreshSession()
            : Task.CompletedTask;
    }
    private static HttpContent? GetContent(object? body)
    {
        return body is null
            ? (HttpContent?)null
            : new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
    }
    private static Task SaveSession(AuthResponseDto dto)
    {
        Preferences.Set(AppShell.SETTING_SESSION, JsonSerializer.Serialize(dto));
        return Task.CompletedTask;
    }
    #endregion

}
