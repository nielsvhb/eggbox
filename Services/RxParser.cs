using Eggbox.Helpers;
using Eggbox.Models;
using Eggbox.Osc;
using OscCore;
using Color = Eggbox.Models.Color;

namespace Eggbox.Services;

public class RxParser
{
    private readonly MixerModel _model;

    public RxParser(MixerModel model)
    {
        _model = model;
    }

    public bool ApplyOscMessage(OscMessage msg, out DateTime parseStart, out DateTime parseEnd)
    {
        parseStart = DateTime.UtcNow;
        var addr = msg.Address;
        var args = msg.Select(a => (object)a).ToArray();

        bool handled =
            HandleChannel(addr, args)
            || HandleBus(addr, args)
            || HandleFx(addr, args)
            || HandleMain(addr, args);

        parseEnd = DateTime.UtcNow;
        return handled;
    }
    
    

    // ----------------------------
    // CHANNEL
    // ----------------------------
    private bool HandleChannel(string addr, object[] args)
    {
        // Fader
        if (OscAddress.Channel.Fader.Match(addr, out int ch))
        {
            var lin = Convert.ToSingle(args[0]);
            var db = DecibelFader.FromLinear(lin);

            _model.Channels[ch - 1].Fader = db;
            _model.MarkInitProgress(addr);

            return true;
        }

        // Mute
        if (OscAddress.Channel.Mute.Match(addr, out ch))
        {
            _model.Channels[ch - 1].Mute = Convert.ToInt32(args[0]) == 0;
            _model.MarkInitProgress(addr);

            return true;
        }

        // Gain
        if (OscAddress.Channel.Gain.Match(addr, out ch))
        {
            var lin = Convert.ToSingle(args[0]);
            var gain = DecibelGain.FromLinear(lin);

            _model.Channels[ch - 1].Gain = gain;
            _model.MarkInitProgress(addr);

            return true;
        }

        // Color
        if (OscAddress.Channel.Color.Match(addr, out ch))
        {
            var colorId = Convert.ToInt32(args[0]);
            _model.Channels[ch - 1].Color = Color.FromMappedValue(colorId);
            _model.MarkInitProgress(addr);

            return true;
        }

        // Name
        if (OscAddress.Channel.Name.Match(addr, out ch))
        {
            _model.Channels[ch - 1].Name = Convert.ToString(args[0]) ?? "-";
            _model.MarkInitProgress(addr);

            return true;
        }

        // Send Level
        if (OscAddress.Channel.SendLevel.Match(addr, out ch, out int bus))
        {
            var lin = Convert.ToSingle(args[0]);
            var db = DecibelFader.FromLinear(lin);

            var send = _model.Channels[ch - 1].GetOrCreateSend(bus);
            send.Level = db;

            _model.MarkInitProgress(addr);

            return true;
        }

        // Send Mute
        if (OscAddress.Channel.SendMute.Match(addr, out ch, out bus))
        {
            var send = _model.Channels[ch - 1].GetOrCreateSend(bus);
            send.Mute = Convert.ToInt32(args[0]) == 0;

            _model.MarkInitProgress(addr);

            return true;
        }

        return false;
    }


    // ----------------------------
    // BUS
    // ----------------------------
    private bool HandleBus(string addr, object[] args)
    {
        if (OscAddress.Bus.Fader.Match(addr, out int bus))
        {
            _model.Busses[bus - 1].Fader = Convert.ToSingle(args[0]);
            _model.MarkInitProgress(addr);

            return true;
        }

        if (OscAddress.Bus.Mute.Match(addr, out bus))
        {
            _model.Busses[bus - 1].Mute = Convert.ToInt32(args[0]) == 0;
            _model.MarkInitProgress(addr);

            return true;
        }

        if (OscAddress.Bus.Color.Match(addr, out bus))
        {
            var colorId = Convert.ToInt32(args[0]);
            var color = Color.FromMappedValue(colorId);
            color.MatchSome(c =>
            {
                _model.Busses[bus - 1].Color = c;
                _model.MarkInitProgress(addr);

            });
          
            return true;
        }

        if (OscAddress.Bus.Name.Match(addr, out bus))
        {
            _model.Busses[bus - 1].Name = Convert.ToString(args[0]) ?? "";
            _model.MarkInitProgress(addr);

            return true;
        }

        return false;
    }

    // ----------------------------
    // FX
    // ----------------------------
    private bool HandleFx(string addr, object[] args)
    {
        // TODO
        // if (OscAddress.Fx.Fader.Match(addr, out int fx))
        // {
        //     (_model.Fx1, _model.Fx2)[fx - 1].Fader = Convert.ToSingle(args[0]);
        //      _model.MarkInitProgress(addr);

        //     return true;
        // }
        //
        // if (OscAddress.Fx.Mute.Match(addr, out fx))
        // {
        //     (_model.Fx1, _model.Fx2)[fx - 1].Mute = Convert.ToInt32(args[0]) == 0;
        //_model.MarkInitProgress(addr);

        //     return true;
        // }

        return false;
    }

    // ----------------------------
    // MAIN LR
    // ----------------------------
    private bool HandleMain(string addr, object[] args)
    {
        if (addr == OscAddress.Main.Fader)
        {
            _model.Main.Fader = Convert.ToSingle(args[0]);
            _model.MarkInitProgress(addr);

            return true;
        }

        if (addr == OscAddress.Main.Mute)
        {
            _model.Main.Mute = Convert.ToInt32(args[0]) == 0;
            _model.MarkInitProgress(addr);

            return true;
        }

        return false;
    }
}
