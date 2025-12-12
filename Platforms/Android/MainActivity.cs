using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls.PlatformConfiguration;


namespace Eggbox;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            var window = Platform.CurrentActivity.Window;
            var uiMode = Resources.Configuration.UiMode & UiMode.NightMask;
            var isDark = uiMode == UiMode.NightYes;
            
            var statusColor = isDark
                ? Android.Graphics.Color.ParseColor("#fbfafa")
                : Android.Graphics.Color.ParseColor("#fbfafa");
            var navColor = isDark
                ? Android.Graphics.Color.ParseColor("#171717")
                : Android.Graphics.Color.ParseColor("#ffffff");
            
            window.SetStatusBarColor(statusColor);
            window.SetNavigationBarColor(navColor);
            
            var flags = window.DecorView.SystemUiVisibility;

            if (isDark)
            {
                flags &= ~(StatusBarVisibility)SystemUiFlags.LightStatusBar;
                flags &= ~(StatusBarVisibility)SystemUiFlags.LightNavigationBar;
            }
            else
            {
                flags |= (StatusBarVisibility)(SystemUiFlags.LightStatusBar | SystemUiFlags.LightNavigationBar);
            }


            window.DecorView.SystemUiVisibility = flags;
        }
    }

}