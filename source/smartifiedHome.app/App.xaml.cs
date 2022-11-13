using smartifiedHome.app.Services.Rest;

namespace smartifiedHome.app;

public partial class App : Application
{
    public App(IApiClient apiClient)
    {
        InitializeComponent();

        MainPage = new AppShell(apiClient);
    }
}
