namespace smartifiedHome.app.Modules.Startup;

public partial class StartupPage : ContentPage
{
    public StartupPage(StartupPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (BindingContext is StartupPageViewModel vm)
        {
            vm.StartupCommand.Execute(null);
        }
    }
}