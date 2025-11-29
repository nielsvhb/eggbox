using Microsoft.Maui.Networking;

namespace Eggbox.Helpers;

public static class WifiService
{
    public static bool IsOnWifi()
        => Connectivity.Current.ConnectionProfiles.Contains(ConnectionProfile.WiFi);

    public static bool HasInternet()
        => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
}