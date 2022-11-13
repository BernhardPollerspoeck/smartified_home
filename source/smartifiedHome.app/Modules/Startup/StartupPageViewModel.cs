using MvvMHelpers.core;
using smartifiedHome.app.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace smartifiedHome.app.Modules.Startup;
public class StartupPageViewModel : BaseViewModel
{
    #region fields
    private readonly IApiClient _apiClient;
    #endregion

    #region properties
    private string _message;
    public string Message
    {
        get => _message;
        set => Set(ref _message, value);
    }

    public ICommand StartupCommand { get; }
    #endregion

    #region ctor
    public StartupPageViewModel(
        IApiClient apiClient)
    {
        _apiClient = apiClient;

        StartupCommand = new AsyncCommand(ExecuteStartup);

        _message = "Starte App";
    }
    #endregion

    #region task handling
    private async Task ExecuteStartup()
    {
        var initResult = await _apiClient.Init();
        var route = initResult switch
        {
            { Expired: true } => "///login",
            { MissingRoute: true } => "///login/route",
            { Success: true } => "///main",

            _ => null,
        };

        if (route is null)
        {
            Message = "Es ist ein Schwerwiegender Fehler aufgetreten.";
            return;
        }

        try
        {
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {

            throw;
        }
    }
    #endregion

}
