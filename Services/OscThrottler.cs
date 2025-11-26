using System.Collections.Concurrent;
using OscCore;

namespace Eggbox.Services;

public sealed class OscThrottler : IAsyncDisposable
{
    private readonly ConcurrentQueue<QueuedMsg> _queue = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _workerTask;
    private readonly TrafficLogger _traffic;

    private readonly int _delayMs;

    public OscThrottler(int delayMs, TrafficLogger traffic)
    {
        _delayMs = delayMs;
        _traffic = traffic;

        _workerTask = Task.Run(WorkLoopAsync);
    }

    private record QueuedMsg(OscMessage Message, Func<Task> Send, DateTime EnqueuedAt);

    public void Enqueue(OscMessage msg, Func<Task> sendFunc)
    {
        _traffic.AddTxQueued(msg);
        _queue.Enqueue(new QueuedMsg(msg, sendFunc, DateTime.UtcNow));
    }
    
    private async Task WorkLoopAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var item))
            {
                try
                {
                    await item.Send();
                    _traffic.AddTxThrottled(item.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MixerIO throttler exception: " + ex);
                }

                await Task.Delay(_delayMs, _cts.Token);
            }
            else
            {
                await Task.Delay(1, _cts.Token);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        try { await _workerTask; } catch {}
        _cts.Dispose();
    }
}