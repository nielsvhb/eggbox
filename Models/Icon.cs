namespace Eggbox.Models;

public class Icon : IntMappedEnumeration<Icon>
{
    protected Icon(string name, int index) : base(name, index) {}

    public static readonly Icon Guitar = new Icon("fa-guitar", 1);
    public static readonly Icon Drum = new Icon("fa-drum", 2);
    public static readonly Icon Microphone = new Icon("fa-microphone", 3);
    public static readonly Icon Keyboard = new Icon("fa-keyboard", 4);
    public static readonly Icon Saxophone = new Icon("fa-saxophone", 5);
    public static readonly Icon CompactDisc = new Icon("fa-compact-disc", 6);
    public static readonly Icon Banjo = new Icon("fa-banjo", 7);
    public static readonly Icon Clarinet = new Icon("fa-clarinet", 8);

}