namespace Gehtsoft.Measurements
{
    public enum VelocityUnit : int
    {
        [Unit("m/s", 0)]
        [Conversion(ConversionOperation.Base)]
        MetersPerSecond = 0,
        
        [Unit("km/h", "kmph", 1)]
        [Conversion(ConversionOperation.Divide, 3.6)]
        KilometersPerHour,
        
        [Unit("ft/s", 1)]
        [Conversion(ConversionOperation.Divide, 3.2808399)]
        FeetPerSecond,
        
        [Unit("mi/h", "mph", 1)]
        [Conversion(ConversionOperation.Divide, 2.23693629)]
        MilesPerHour,
        
        [Unit("kt", 1)]
        [Conversion(ConversionOperation.Divide, 1.94384449)]
        Knot,
    }
}
