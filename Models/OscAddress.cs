namespace Eggbox.Osc;

public static class OscAddress
{
    public static class Channel
    {
        public static readonly OscPattern Fader     = new("/ch/{0:D2}/mix/fader");
        public static readonly OscPattern Mute      = new("/ch/{0:D2}/mix/on");
        public static readonly OscPattern Gain      = new("/headamp/{0:D2}/gain");
        public static readonly OscPattern Color     = new("/ch/{0:D2}/config/color");
        public static readonly OscPattern Name     = new("/ch/{0:D2}/config/name");

        public static readonly OscPattern SendLevel = new("/ch/{0:D2}/mix/{1:D2}/level");
    }

    public static class Bus
    {
        public static readonly OscPattern Fader = new("/bus/{0}/mix/fader");
        public static readonly OscPattern Mute  = new("/bus/{0}/mix/on");
        public static readonly OscPattern Color = new("/bus/{0}/config/color");
        public static readonly OscPattern Name  = new("/bus/{0}/config/name");
    }

    public static class Fx
    {
        public static readonly OscPattern Fader = new("/fxr/{0}/mix/fader");
        public static readonly OscPattern Mute  = new("/fxr/{0}/mix/on");
    }

    public static class Main
    {
        public const string Fader = "/lr/mix/fader";
        public const string Mute  = "/lr/mix/on";
    }
}