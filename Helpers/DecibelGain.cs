namespace Eggbox.Helpers;

public readonly struct DecibelGain
{
    public double Db { get; }

    public DecibelGain(double db)
    {
        Db = Math.Clamp(db, -12, 60);
    }
    
    public override string ToString()
        => $"{Db:0.#} dB";

    public double ToLinear() => (Db + 12) / 72;

    public static DecibelGain FromLinear(double lin) => lin * 72 - 12;

    public static implicit operator DecibelGain(double db) => new (db);

    public static implicit operator double(DecibelGain d) => d.Db;
}
