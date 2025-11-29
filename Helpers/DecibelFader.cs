using System.Globalization;

namespace Eggbox.Helpers;

public readonly struct DecibelFader
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    public double Db { get; }

    public DecibelFader(double db)
    {
        Db = Math.Clamp(db, -90, 0);
    }
    
    public override string ToString()
        => Db.ToString("0.#", Invariant);
    
    public double ToLinear()
    {
        if (Db <= -90) return 0.0;

        if (Db < -30)
            return (Db + 90.0) / 300.0;

        if (Db < -10)
            return 0.20 + (Db + 30.0) / 80.0;

        if (Db < 0)
            return 0.75 + (Db + 10.0) / 13.3;

        return 1.0;
    }

    public static DecibelFader FromLinear(double lin)
    {
        if (lin <= 0.0) return -90.0;

        if (lin < 0.20)
            return lin * 300.0 - 90.0;

        if (lin < 0.75)
            return (lin - 0.20) * 80.0 - 30.0;

        if (lin < 1.0)
            return (lin - 0.75) * 13.3 - 10.0;

        return 0.0;
    }

    public static implicit operator DecibelFader(double db) => new (db);

    public static implicit operator double(DecibelFader d) => d.Db;
}
