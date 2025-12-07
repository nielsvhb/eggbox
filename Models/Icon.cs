namespace Eggbox.Models;

public class Icon : IntMappedEnumeration<Icon>
{
    protected Icon(string name, int index) : base(name, index) {}

    public static readonly Icon Guitar = new Icon("fa-guitar", 1);
    public static readonly Icon Saxophone = new Icon("fa-guitar-electric", 2);
    public static readonly Icon Banjo = new Icon("fa-guitars", 3);
    public static readonly Icon Drum = new Icon("fa-drum", 4);
    public static readonly Icon Microphone = new Icon("fa-microphone", 5);
    public static readonly Icon Keyboard = new Icon("fa-piano-keyboard", 6);
    public static readonly Icon CompactDisc = new Icon("fa-music-note", 7);

}