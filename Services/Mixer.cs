using Eggbox.Helpers;
using Eggbox.Models;
using Eggbox.Osc;
using OscCore;
using Color = Eggbox.Models.Color;

namespace Eggbox.Services;

public class Mixer
{
    private readonly MixerModel _model;
    private readonly MixerIO _io;
    private readonly TrafficLogger _traffic;

    public Mixer(
        MixerModel model,
        MixerIO io,
        TrafficLogger traffic)
    {
        _model = model;
        _io = io;
        _traffic = traffic;

        _io.MessageSent += OnSent;
    }

    private void OnSent(OscMessage msg, DateTime t)
    {
        _traffic.AddTx(msg);
    }

    public async Task ConnectAsync(MixerInfo info)
    {
        if (info.IpAddress is null)
            throw new ArgumentException("MixerInfo.IpAddress cannot be null");

        await _io.ConnectAsync(info.IpAddress);

        _model.IsConnected = true;
        _model.IpAddress = info.IpAddress;

        Configure(info.MixerType);

        await InitAsync();
    }

    public MixerModel State => _model;
    
    public Mixer Configure(string mixerType)
    {
        _model.Info.MixerType = mixerType.ToLowerInvariant();

        switch (_model.Info.MixerType)
        {
            case "xr18":
                _model.Info.ChannelCount = 18;
                _model.Info.BusCount = 6;
                break;
            case "xr16":
            default:
                _model.Info.ChannelCount = 16;
                _model.Info.BusCount = 4;
                break;
        }

        _model.Channels = Enumerable.Range(1, _model.Info.ChannelCount)
            .Select(i => new Channel { Index = i, Name = $"CH{i:00}" })
            .ToList();

        _model.Busses = Enumerable.Range(1, _model.Info.BusCount)
            .Select(i => new Bus { Index = i, Name = $"BUS{i:00}" })
            .ToList();

        return this;
    }
    
    public async Task InitAsync()
    {
        var chCount = _model.Info.ChannelCount;
        var busCount = _model.Info.BusCount;

        var tasks = new List<Task>();

        for (int ch = 1; ch <= chCount; ch++)
            tasks.Add(Channel(ch).RequestRefreshAsync());

        for (int bus = 1; bus <= busCount; bus++)
            tasks.Add(Bus(bus).RequestRefreshAsync());

        tasks.Add(Fx(1).RequestRefreshAsync());
        tasks.Add(Fx(2).RequestRefreshAsync());
        tasks.Add(Main().RequestRefreshAsync());

        await Task.WhenAll(tasks);
    }

    public ChannelControl Channel(int index) => new(_io, index);
    public BusControl Bus(int index) => new(_io, index);
    public FxControl Fx(int index) => new(_io, index);
    public MainControl Main() => new(_io, _model);

    public async Task ReloadChannels()
    {
        for (int i = 1; i <= _model.Info.ChannelCount; i++)
        {
            await Channel(i).RequestRefreshAsync();
        }
    }
}

public class ChannelControl
{
    private readonly MixerIO _io;
    private readonly int _index;

    public ChannelControl(MixerIO io, int index)
    {
        _io = io;
        _index = index;
    }

    /*
     * Volume of Channel to Main Mix
     */
    public Task SetFader(DecibelFader db)
    {
        return _io.SendMessage(OscAddress.Channel.Fader.Build(_index), db);
    }

    public Task SetMute(bool muted)
        => _io.SendMessage(OscAddress.Channel.Mute.Build(_index), muted ? 0 : 1);

    public Task SetGain(DecibelGain db)
    {
        return _io.SendMessage(OscAddress.Channel.Gain.Build(_index), db);
    }

    public Task SetColor(Color color)
        => _io.SendMessage(OscAddress.Channel.Color.Build(_index), color.MappedValue);
    public Task SetName(string name)
        => _io.SendMessage(OscAddress.Channel.Name.Build(_index), name);

    public Task SetSendLevel(int bus, float value)
        => _io.SendMessage(OscAddress.Channel.SendLevel.Build(_index, bus), value);
   
    public Task SetSendMute(int bus, bool mute)
        => _io.SendMessage(OscAddress.Channel.SendMute.Build(_index, bus), mute ? 0 : 1);

    public Task RequestRefreshAsync()
        => Task.WhenAll(
            _io.SendMessage(OscAddress.Channel.Fader.Build(_index)),
            _io.SendMessage(OscAddress.Channel.Mute.Build(_index)),
            _io.SendMessage(OscAddress.Channel.Gain.Build(_index)),
            _io.SendMessage(OscAddress.Channel.Color.Build(_index)),
            _io.SendMessage(OscAddress.Channel.Name.Build(_index))
        );
}


public class BusControl
{
    private readonly MixerIO _io;
    private readonly int _bus;

    public BusControl(MixerIO io, int bus)
    {
        _io = io;
        _bus = bus;
    }

    public Task SetFader(DecibelFader value)
        => _io.SendMessage(OscAddress.Bus.Fader.Build(_bus), value);

    public Task SetMute(bool mute)
        => _io.SendMessage(OscAddress.Bus.Mute.Build(_bus), mute ? 0 : 1);

    public Task SetName(string name)
        => _io.SendMessage(OscAddress.Bus.Name.Build(_bus), name);

    public Task SetColor(Color color)
        => _io.SendMessage(OscAddress.Bus.Color.Build(_bus), color.MappedValue);

    public Task RequestRefreshAsync()
        => Task.WhenAll(
            _io.SendMessage(OscAddress.Bus.Fader.Build(_bus)),
            _io.SendMessage(OscAddress.Bus.Mute.Build(_bus)),
            _io.SendMessage(OscAddress.Bus.Name.Build(_bus)),
            _io.SendMessage(OscAddress.Bus.Color.Build(_bus))
        );
}

public class MainControl
{
    private readonly MixerIO _io;

    public MainControl(MixerIO io, MixerModel model)
    {
        _io = io;
    }

    public Task SetFader(DecibelFader value)
        => _io.SendMessage(OscAddress.Main.Fader, value);

    public Task SetMute(bool mute)
        => _io.SendMessage(OscAddress.Main.Mute, mute ? 0 : 1);

    public Task RequestRefreshAsync()
        => Task.WhenAll(
            _io.SendMessage(OscAddress.Main.Fader),
            _io.SendMessage(OscAddress.Main.Mute)
        );
}

public class FxControl
{
    private readonly MixerIO _io;
    private readonly int _index;

    public FxControl(MixerIO io, int index)
    {
        _io = io;
        _index = index;
    }

    public Task SetReturnFader(DecibelFader value)
        => _io.SendMessage(OscAddress.Fx.Fader.Build(_index), value);

    public Task SetMute(bool mute)
        => _io.SendMessage(OscAddress.Fx.Mute.Build(_index), mute ? 0 : 1);

    public Task RequestRefreshAsync()
    {
        return Task.WhenAll(
                _io.SendMessage(OscAddress.Fx.Fader.Build(_index)),
                _io.SendMessage(OscAddress.Fx.Mute.Build(_index))
        );
    }
}
