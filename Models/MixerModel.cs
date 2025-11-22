namespace Eggbox.Models;

public class MixerModel
{
    public MixerInfo Info { get; set; } = new();
    public List<Channel> Channels { get; set; } = new();
    public List<Bus> Busses { get; set; } = new();
    public FxReturn Fx1 { get; set; } = new() { Index = 1 };
    public FxReturn Fx2 { get; set; } = new() { Index = 2 };
    public MainBus Main { get; set; } = new();

    public bool IsConnected { get; set; }
    public string? IpAddress { get; set; }

    public event Action<string>? StateChanged;
    public void RaiseStateChanged(string key) => StateChanged?.Invoke(key);
}

public class MixerInfo
{
    public string MixerType { get; set; } = "xr16";
    public string? Name { get; set; }
    public string? FirmwareVersion { get; set; }
    public int ChannelCount { get; set; }
    public int BusCount { get; set; }
    public string? IpAddress { get; set; } 
}

public class Channel
{
    public int Index { get; set; }
    public string Name { get; set; } = "";
    public float Fader { get; set; }
    public bool Mute { get; set; }
    public float Gain { get; set; }
    public MixerColor Color { get; set; } = MixerColor.Red;
    public Dictionary<int, ChannelSend> Sends { get; set; } = new();
}

public class ChannelSend
{
    public float Level { get; set; }
    public bool Mute { get; set; }
}


public class Bus
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public float Fader { get; set; }
    public bool Mute { get; set; }
    public float[] Meters { get; set; } = Array.Empty<float>();
    public MixerColor Color { get; set; } = MixerColor.Red;
}

public class FxReturn
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public float Fader { get; set; }
    public bool Mute { get; set; }
}

public class MainBus
{
    public float Fader { get; set; }
    public bool Mute { get; set; }
    public float[] Meters { get; set; } = Array.Empty<float>();
}