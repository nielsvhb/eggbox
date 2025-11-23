using Eggbox.Models;
using Eggbox.Osc;
using Optional;
using OscCore;
using Color = Eggbox.Models.Color;

namespace Eggbox.Services;

public class Mixer
{
    private readonly MixerModel _model;
    private readonly UDPService _io;
    private readonly RxParser _parser;
    private readonly TrafficLogger _traffic;

    public Mixer(
        MixerModel model,
        UDPService io,
        RxParser parser,
        TrafficLogger traffic)
    {
        _model = model;
        _io = io;
        _parser = parser;
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

    internal Task SendAsync(OscMessage msg) => _io.SendAsync(msg);


    public ChannelControl Channel(int index) => new(_io, _model, index);
    public BusControl Bus(int index) => new(_io, _model, index);
    public FxControl Fx(int index) => new(this, _model, index);
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
    private readonly UDPService _io;
    private readonly MixerModel _model;
    private readonly int _index;

    public ChannelControl(UDPService io, MixerModel model, int index)
    {
        _io = io;
        _model = model;
        _index = index;
    }

    public Task SetFader(float value)
        => _io.SendAsync(new OscMessage(OscAddress.Channel.Fader.Build(_index), value));

    public Task SetMute(bool muted)
        => _io.SendAsync(new OscMessage(OscAddress.Channel.Mute.Build(_index), muted ? 0 : 1));

    public Task SetGain(float db)
    {
        double linear = GainConverter.DbToLinear(db);
        return _io.SendAsync(
            new OscMessage(OscAddress.Channel.Gain.Build(_index), (float)linear)
        );
    }

    public Task SetColor(Color color)
        => _io.SendAsync(new OscMessage(OscAddress.Channel.Color.Build(_index), color.MappedValue));
    public Task SetName(string name)
        => _io.SendAsync(new OscMessage(OscAddress.Channel.Name.Build(_index), name));

    public Task SetSendLevel(int bus, float value)
        => _io.SendAsync(new OscMessage(OscAddress.Channel.SendLevel.Build(_index, bus), value));

    public Task SetSendMute(int bus, bool mute)
        => _io.SendAsync(new OscMessage(OscAddress.Channel.SendMute.Build(_index, bus), mute ? 0 : 1));

    public Task RequestRefreshAsync()
        => Task.WhenAll(
            _io.SendAsync(new OscMessage(OscAddress.Channel.Fader.Build(_index))),
            _io.SendAsync(new OscMessage(OscAddress.Channel.Mute.Build(_index))),
            _io.SendAsync(new OscMessage(OscAddress.Channel.Gain.Build(_index))),
            _io.SendAsync(new OscMessage(OscAddress.Channel.Color.Build(_index))),
            _io.SendAsync(new OscMessage(OscAddress.Channel.Name.Build(_index)))
        );
}


public class BusControl
{
    private readonly UDPService _io;
    private readonly MixerModel _model;
    private readonly int _bus;

    public BusControl(UDPService io, MixerModel model, int bus)
    {
        _io = io;
        _model = model;
        _bus = bus;
    }

    public Task SetFader(float value)
        => _io.SendAsync(new OscMessage(OscAddress.Bus.Fader.Build(_bus), value));

    public Task SetMute(bool mute)
        => _io.SendAsync(new OscMessage(OscAddress.Bus.Mute.Build(_bus), mute ? 0 : 1));

    public Task SetName(string name)
        => _io.SendAsync(new OscMessage(OscAddress.Bus.Name.Build(_bus), name));

    public Task SetColor(Color color)
        => _io.SendAsync(new OscMessage(OscAddress.Bus.Color.Build(_bus), color.MappedValue));

    public Task RequestRefreshAsync()
        => Task.WhenAll(
            _io.SendAsync(new OscMessage(OscAddress.Bus.Fader.Build(_bus))),
            _io.SendAsync(new OscMessage(OscAddress.Bus.Mute.Build(_bus))),
            _io.SendAsync(new OscMessage(OscAddress.Bus.Name.Build(_bus))),
            _io.SendAsync(new OscMessage(OscAddress.Bus.Color.Build(_bus)))
        );

    public ChannelSendControl Channel(int ch)
        => new (_io, _model, ch, _bus);
}

public class ChannelSendControl
{
    private readonly UDPService _io;
    private readonly MixerModel _model;
    private readonly int _ch;
    private readonly int _bus;

    public ChannelSendControl(UDPService io, MixerModel model, int ch, int bus)
    {
        _io = io;
        _model = model;
        _ch = ch;
        _bus = bus;
    }

    private ChannelSend? SendModel
    {
        get
        {
            var ch = _model.Channels.FirstOrDefault(c => c.Index == _ch);
            if (ch == null) return null;
            return ch.Sends.TryGetValue(_bus, out var s) ? s : null;
        }
    }

    public float Level => SendModel?.Level ?? 0f;
    public bool Mute => SendModel?.Mute ?? false;

    public Task SetFader(float value)
        => _io.SendAsync(new OscMessage($"/ch/{_ch:D2}/mix/{_bus:D2}/level", value));

    public Task SetMute(bool mute)
        => _io.SendAsync(new OscMessage($"/ch/{_ch:D2}/mix/{_bus:D2}/on", mute ? 0 : 1));
}

public class MainControl
{
    private readonly UDPService _io;
    private readonly MixerModel _model;

    public MainControl(UDPService io, MixerModel model)
    {
        _io = io;
        _model = model;
    }

    public Task SetFader(float value)
        => _io.SendAsync(new OscMessage(OscAddress.Main.Fader, value));

    public Task SetMute(bool mute)
        => _io.SendAsync(new OscMessage(OscAddress.Main.Mute, mute ? 0 : 1));

    public Task RequestRefreshAsync()
        => Task.WhenAll(
            _io.SendAsync(new OscMessage(OscAddress.Main.Fader)),
            _io.SendAsync(new OscMessage(OscAddress.Main.Mute))
        );
    
    public ChannelControl Channel(int index)
        => new ChannelControl(_io, _model, index);
}

public class FxControl
{
    private readonly Mixer _mixer;
    private readonly MixerModel _model;
    private readonly int _index;

    public FxControl(Mixer mixer, MixerModel model, int index)
    {
        _mixer = mixer;
        _model = model;
        _index = index;
    }

    private FxReturn FxModel
        => _index == 1 ? _model.Fx1 : _model.Fx2;

    public float Fader => FxModel.Fader;
    public bool Mute => FxModel.Mute;
    public string Name => FxModel.Name;

    public Task SetReturnFader(float value)
        => _mixer.SendAsync(new OscMessage($"/fxr/{_index}/mix/fader", value));

    public Task SetMute(bool mute)
        => _mixer.SendAsync(new OscMessage($"/fxr/{_index}/mix/on", mute ? 0 : 1));

    public Task RequestRefreshAsync()
    {
        return Task.WhenAll(
            _mixer.SendAsync(new OscMessage($"/fxr/{_index}/mix/fader")),
            _mixer.SendAsync(new OscMessage($"/fxr/{_index}/mix/on"))
        );
    }
}
