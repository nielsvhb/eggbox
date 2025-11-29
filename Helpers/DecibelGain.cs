using System.Globalization;

namespace Eggbox.Helpers;

public readonly struct DecibelGain
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    public double Db { get; }

    public DecibelGain(double db)
    {
        Db = Math.Clamp(db, -12, 60);
    }
    
    public override string ToString()
        => Db.ToString("0.#", Invariant);
    
    public double ToLinear() => (Db + 12) / 72;
    
    public static DecibelGain FromLinear(double lin)
        => lin * 72 - 12;
    
    public static implicit operator DecibelGain(double db)
        => new(db);

    public static implicit operator double(DecibelGain d)
        => d.Db;
}