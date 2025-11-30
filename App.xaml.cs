namespace Eggbox;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
    
    protected override void OnStart()
    {
        Microsoft.Maui.Controls.Application.Current.Resources["WindowStatusBarColor"] = Color.FromArgb("#292524");
    }

}