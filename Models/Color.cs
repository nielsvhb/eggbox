namespace Eggbox.Models;

public class Color : IntMappedEnumeration<Color>
{
    public readonly string HexCode;
    private Color(string name, string value, int index, string hexCode) : base(name, index)
    {
        HexCode = hexCode;
    }

    public static readonly Color Red = new (nameof(Red), "RD", 1, "#ff6464");
    public static readonly Color Green = new (nameof(Green), "GN", 2, "#2dc75c");
    public static readonly Color Yellow = new (nameof(Yellow), "YE", 3, "#ffdc4a");
    public static readonly Color Blue = new (nameof(Blue), "BL", 4, "#659df2");
    public static readonly Color Magenta = new (nameof(Magenta), "MG", 5, "#8f7fee");
    public static readonly Color White = new (nameof(White), "WH", 6, "#ffffff");
}