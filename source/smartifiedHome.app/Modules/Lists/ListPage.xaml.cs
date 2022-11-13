namespace smartifiedHome.app.Modules.Lists;

public partial class ListPage : ContentPage
{
    public ListPage(ListPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}