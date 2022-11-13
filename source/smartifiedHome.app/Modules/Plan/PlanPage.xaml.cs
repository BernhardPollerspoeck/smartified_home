namespace smartifiedHome.app.Modules.Plan;

public partial class PlanPage : ContentPage
{
    public PlanPage(PlanPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}