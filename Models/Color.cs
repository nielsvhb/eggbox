namespace Eggbox.Models;

public class Color : IntMappedEnumeration<Color>
{
    public readonly string HexOpaque;
    public readonly string HexTransparent;
    private Color(string name, string value, int index, string hexOpaque, string hexTransparent) : base(name, index)
    {
        HexOpaque = hexOpaque;
        HexTransparent = hexTransparent;
    }

    public static readonly Color Red = new (nameof(Red), "RD", 1, "#ff6464", "#ff646420");
    public static readonly Color Green = new (nameof(Green), "GN", 2, "#2dc75c", "#2dc75c20");
    public static readonly Color Yellow = new (nameof(Yellow), "YE", 3, "#ffdc4a", "#ffdc4a20");
    public static readonly Color Blue = new (nameof(Blue), "BL", 4, "#659df2", "#659df220");
    public static readonly Color Magenta = new(nameof(Magenta), "MG", 5, "#8f7fee", "#8f7fee20"); 
    public static readonly Color White = new (nameof(White), "WH", 6, "#ffffff", "#ffffff");
}