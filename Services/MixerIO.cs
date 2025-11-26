using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using OscCore;

namespace Eggbox.Services;

public sealed class MixerIO : IAsyncDisposable
{
    private readonly ILogger<MixerIO> _logger;
    private UdpClient? _client;
    private RxParser _parser;
    private TrafficLogger _traffic;
    private CancellationTokenSource? _cts;
    private Timer? _subscriptionTimer;
    private IPEndPoint? _remoteEndPoint;
    private readonly OscThrottler _throttler;

    public int LocalPort { get; private set; }

    private const int DefaultLocalPort = 10025;

    public MixerIO(ILogger<MixerIO> logger, RxParser parser, TrafficLogger traffic)
    {
        _logger = logger;
        _parser = parser;
        _traffic = traffic;
        _throttler = new OscThrottler(15, traffic);

    }

    public async Task ConnectAsync(string hostName, int port = 10024, int? localPort = null)
    {
        await DisconnectAsync();

        LocalPort = localPort ?? DefaultLocalPort;
        _client = new UdpClient(LocalPort);
        _remoteEndPoint = new IPEndPoint(IPAddress.Parse(hostName), port);

        _logger.LogInformation("UDP client bound to local endpoint {EP}", _client.Client.LocalEndPoint);

        _cts = new CancellationTokenSource();
        _ = StartReceivingAsync(_cts.Token);

        _ = SendMessage("/xremote");
        _ = SendMessage("/xinfo");

        _subscriptionTimer = new Timer(_ =>
        {
            _ = SendMessage("/xremote");
        }, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

        await Task.CompletedTask;
    }

    private async Task SendAsync(OscMessage msg)
    {
        if (_client == null || _remoteEndPoint == null)
            throw new InvalidOperationException("UDPService is not connected.");

        var packet = (OscPacket)msg;
        var data = packet.ToByteArray();
        var sentAt = DateTime.UtcNow;

        await _client.SendAsync(data, data.Length, _remoteEndPoint).ConfigureAwait(false);
    }

    private Task StartReceivingAsync(CancellationToken token) => Task.Run(async () =>
    {
        if (_client == null) return;

        _logger.LogInformation("📡 Start receiving on local port {Port}", LocalPort);
        try
        {
            while (!token.IsCancellationRequested)
            {
                var result = await _client.ReceiveAsync(token).ConfigureAwait(false);
                var buffer = result.Buffer;
                var packet = OscPacket.Read(buffer, 0, buffer.Length);
                var rxTime = DateTime.UtcNow;

                if (packet is OscMessage msg)
                {
                    bool handled = _parser.ApplyOscMessage(msg,
                        out var parseStart,
                        out var parseEnd);
                    _traffic.AddRx(msg, handled, rxTime, parseStart, parseEnd);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // normaal bij disconnect
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fout tijdens ontvangen van OSC");
        }
    });

    public async Task DisconnectAsync()
    {
        _cts?.Cancel();
        _cts = null;

        _subscriptionTimer?.Dispose();
        _subscriptionTimer = null;

        _client?.Dispose();
        _client = null;

        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
    
    public Task SendMessage(string address, object? value = null)
    {
        OscMessage Make()
        {
            if (value is null) return new OscMessage(address);
            if (value is DecibelGain g) return new OscMessage(address, (float)g.ToLinear());
            if (value is DecibelFader f) return new OscMessage(address, (float)f.ToLinear());
            if (value is float fl) return new OscMessage(address, fl);
            if (value is double d) return new OscMessage(address, (float)d);
            if (value is int i) return new OscMessage(address, i);
            if (value is string s) return new OscMessage(address, s);
            throw new InvalidOperationException($"Unsupported OSC argument type: {value.GetType()}");
        }

        var msg = Make();

        _throttler.Enqueue(msg, () => SendAsync(msg));
        return Task.CompletedTask;
    }

}
