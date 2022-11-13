using Microsoft.Extensions.Logging;
using smartifiedHome.app.Modules.Lists;
using smartifiedHome.app.Modules.Login;
using smartifiedHome.app.Modules.Plan;
using smartifiedHome.app.Modules.Route;
using smartifiedHome.app.Modules.Settings;
using smartifiedHome.app.Modules.Startup;
using smartifiedHome.app.Services.Rest;

namespace smartifiedHome.app;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IApiClient, ApiClient>();


        builder.Services.AddTransient<StartupPage>();
        builder.Services.AddTransient<StartupPageViewModel>();

        builder.Services.AddTransient<RoutePage>();
        builder.Services.AddTransient<RoutePageViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginPageViewModel>();

        builder.Services.AddTransient<PlanPage>();
        builder.Services.AddTransient<PlanPageViewModel>();

        builder.Services.AddTransient<ListPage>();
        builder.Services.AddTransient<ListPageViewModel>();

        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<SettingsPageViewModel>();

        return builder.Build();
    }
}
