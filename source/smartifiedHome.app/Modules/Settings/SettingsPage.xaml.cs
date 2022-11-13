namespace smartifiedHome.app.Modules.Settings;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}