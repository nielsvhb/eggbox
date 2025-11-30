public readonly struct DecibelFader
{
    private const double MinDb = -90;
    private const double MaxDb = 10;
    private static readonly (double db, double lin)[] Map =
    {
        (-90, 0.000),
        (-50, 0.125),
        (-30, 0.250),
        (-20, 0.375),
        (-10, 0.500),
        (-5,  0.625),
        (0,   0.750),
        (5,   0.875),
        (10,  1.000)
    };

    public double Db { get; }

    public DecibelFader(double db)
    {
        Db = Math.Clamp(db, -90, 10);
    }

    public double ToLinear()
    {
        // boundaries
        if (Db <= MinDb) return 0;
        if (Db >= MaxDb) return 1;

        // find surrounding points
        for (var i = 0; i < Map.Length - 1; i++)
        {
            var (db1, lin1) = Map[i];
            var (db2, lin2) = Map[i + 1];

            if (Db >= db1 && Db <= db2)
            {
                var t = (Db - db1) / (db2 - db1);
                return lin1 + t * (lin2 - lin1);
            }
        }

        return 0; // unreachable
    }

    public static DecibelFader FromLinear(double linear)
    {
        linear = Math.Clamp(linear, 0, 1);

        // find surrounding points
        for (var i = 0; i < Map.Length - 1; i++)
        {
            var (db1, lin1) = Map[i];
            var (db2, lin2) = Map[i + 1];

            if (linear >= lin1 && linear <= lin2)
            {
                var t = (linear - lin1) / (lin2 - lin1);
                return new DecibelFader(db1 + t * (db2 - db1));
            }
        }

        return new DecibelFader(-90); // fallback
    }
    
    public override string ToString()
        => Db <= MinDb ? "-∞" : Db.ToString("0.#");
    
    public static implicit operator DecibelFader(double db) => new(db);
    public static implicit operator double(DecibelFader d) => d.Db;
}