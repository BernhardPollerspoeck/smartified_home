using smart.contract;
using smartifiedHome.app.Services.Rest.Models;

namespace smartifiedHome.app.Services.Rest;
public interface IApiClient
{

    Task<InitResult> Init();


    Task<ApiResult<AuthResponseDto>> Authenticate(string username, string password);
    Task<ApiResult<AuthResponseDto>> RefreshSession();
    Task Logout();
}
