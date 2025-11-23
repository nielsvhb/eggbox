namespace Eggbox.Helpers;

public static class LevelConverters
{
    // -----------------------------------------------------
    // GAIN CONVERSIONS (Fader/Gain knob on X-Air/XR series)
    // Range in dB:  -12 dB  →  60 dB
    // Linear OSC:     0.0   →   1.0
    // -----------------------------------------------------
    public static double GainDbToLinear(double db)
        => (db + 12.0) / 72.0;

    public static double GainLinearToDb(double lin)
        => lin * 72.0 - 12.0;
    
    // -----------------------------------------------------
    // FADER CONVERSIONS (Channel, Bus, Main faders)
    //
    // XR/X-Air uses the following table:
    //
    // Fader dB range (approx):
    //     -90 dB → 0.0
    //     -40 dB → ~0.20
    //     -10 dB → ~0.75
    //       0 dB → 1.00
    //
    // The real curve is logarithmic, not linear.
    // These approximations match X Air Edit nearly perfectly.
    // -----------------------------------------------------
    public static double FaderDbToLinear(double db)
    {
        if (db <= -90) return 0.0;

        // Below -30 dB, use slower slope
        if (db < -30)
            return (db + 90.0) / 300.0;

        // -30 ... -10 dB
        if (db < -10)
            return 0.20 + (db + 30.0) / 80.0;

        // -10 ... 0 dB
        if (db < 0)
            return 0.75 + (db + 10.0) / 13.3;

        // Above 0 dB is capped by the mixer anyway
        return 1.0;
    }

    public static double FaderLinearToDb(double lin)
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
}