namespace smartifiedHome.app.Modules.Route;

public partial class RoutePage : ContentPage
{
    public RoutePage(RoutePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}