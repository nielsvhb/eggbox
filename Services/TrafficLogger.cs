using System;
using System.Collections.ObjectModel;
using System.Linq;
using Eggbox.Models;
using Microsoft.Maui.Dispatching;
using OscCore;

namespace Eggbox.Services;

public class TrafficLogger
{
    public ObservableCollection<TrafficLogEntry> Log { get; } = new();

    public void AddTx(OscMessage msg)
    {
        var args = msg.Select(a => (object)a).ToArray();

        var entry = new TrafficLogEntry(
            Timestamp: DateTime.UtcNow,
            IsTx: true,
            Address: msg.Address,
            Arguments: args,
            Handled: true,
            RxTime: null,
            ParseStart: null,
            ParseEnd: null
        );

        Add(entry);
    }

    public void AddRx(
        OscMessage msg,
        bool handled,
        DateTime rxTime,
        DateTime parseStart,
        DateTime parseEnd)
    {
        var args = msg.Select(a => (object)a).ToArray();

        var entry = new TrafficLogEntry(
            Timestamp: DateTime.UtcNow,
            IsTx: false,
            Address: msg.Address,
            Arguments: args,
            Handled: handled,
            RxTime: rxTime,
            ParseStart: parseStart,
            ParseEnd: parseEnd
        );

        Add(entry);
    }

    private void Add(TrafficLogEntry logEntry)
    {
        MainThread.BeginInvokeOnMainThread(() => Log.Add(logEntry));
    }
}