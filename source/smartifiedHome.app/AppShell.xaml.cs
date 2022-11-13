using MvvMHelpers.core;
using smartifiedHome.app.Modules.Route;
using smartifiedHome.app.Services.Rest;
using System.Windows.Input;

namespace smartifiedHome.app;

public partial class AppShell : Shell
{
    #region const 
    public const string SETTING_URL = "baseUrl";
    public const string SETTING_SESSION = "session";
    #endregion

    private readonly IApiClient _apiClient;

    public AppShell(IApiClient apiClient)
    {
        InitializeComponent();
        _apiClient = apiClient;

        Routing.RegisterRoute("route", typeof(RoutePage));
    }


    public ICommand LogoutCommand => new AsyncCommand(ExecuteLogout);

    private async Task ExecuteLogout()
    {
        await _apiClient.Logout();
        await GoToAsync("///startup");
    }


}
