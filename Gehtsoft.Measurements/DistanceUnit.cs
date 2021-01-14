namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The units
    /// </summary>
    public enum DistanceUnit : int
    {
        [Unit("ln", 1)]
        [Conversion(ConversionOperation.Divide, 10)]
        RussianLine,
        [Unit("\"", "in", 1)]
        [Conversion(ConversionOperation.Base)]
        Inch,
        [Unit("\'", "ft", 2)]
        [Conversion(ConversionOperation.Multiple, 12)]
        Foot,
        [Unit("yd", 2)]
        [Conversion(ConversionOperation.Multiple, 36)]
        Yard,
        [Unit("mi", 3)]
        [Conversion(ConversionOperation.Multiple, 63360)]
        Mile,
        [Unit("nm", 3)]
        [Conversion(ConversionOperation.Multiple, 72913.3858)]
        NauticalMile,
        [Unit("mm", 0)]
        [Conversion(ConversionOperation.Divide, 25.4)]
        Millimeter,
        [Unit("cm", 1)]
        [Conversion(ConversionOperation.Divide, 2.54)]
        Centimeter,
        [Unit("m", 1)]
        [Conversion(ConversionOperation.Divide, 25.4, ConversionOperation.Multiple, 1000)]
        Meter,
        [Unit("km", 3)]
        [Conversion(ConversionOperation.Divide, 25.4, ConversionOperation.Multiple, 1_000_000)]
        Kilometer,
    }
}
