public static class GainConverter
{
    public static double DbToLinear(double db)
    {
        double scaled = db switch
        {
            >= -5   => (db + 5)  * 20     + 500, // 100/5
            >= -10  => (db + 10) * 20     + 400,
            >= -20  => (db + 20) * 10     + 300,
            >= -30  => (db + 30) * 10     + 200,
            >= -50  => (db + 50) * 5      + 100,
            >= -75  => (db + 75) * 4      + 0,   // 100/25 = 4
            _       => 0
        };

        return scaled / 600.0;
    }

    public static double LinearToDb(double lin)
    {
        double scaled = lin * 600.0;

        // reverse the segmentation
        return scaled switch
        {
            >= 500 => (scaled - 500) / 20.0 - 5,
            >= 400 => (scaled - 400) / 20.0 - 10,
            >= 300 => (scaled - 300) / 10.0 - 20,
            >= 200 => (scaled - 200) / 10.0 - 30,
            >= 100 => (scaled - 100) / 5.0  - 50,
            >= 0   => (scaled - 0)   / 4.0  - 75,
            _      => -999 // unreachable
        };
    }
}