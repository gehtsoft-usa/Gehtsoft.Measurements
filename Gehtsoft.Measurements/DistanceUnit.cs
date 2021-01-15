namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The units
    /// </summary>
    public enum DistanceUnit : int
    {
        [Unit("ln", 1)]
        [Conversion(ConversionOperation.Divide, 12)]
        Line,
        [Unit("rln", 1)]
        [Conversion(ConversionOperation.Divide, 10)]
        RussianLine,
        [Unit("\"", "in", 1)]
        [Conversion(ConversionOperation.Base)]
        Inch,
        [Unit("\'", "ft", 2)]
        [Conversion(ConversionOperation.Multiply, 12)]
        Foot,
        [Unit("yd", 2)]
        [Conversion(ConversionOperation.Multiply, 36)]
        Yard,
        [Unit("mi", 3)]
        [Conversion(ConversionOperation.Multiply, 63360)]
        Mile,
        [Unit("nm", 3)]
        [Conversion(ConversionOperation.Multiply, 1_852_000, ConversionOperation.Divide, 25.4)]
        NauticalMile,
        [Unit("mm", 0)]
        [Conversion(ConversionOperation.Divide, 25.4)]
        Millimeter,
        [Unit("cm", 1)]
        [Conversion(ConversionOperation.Divide, 2.54)]
        Centimeter,
        [Unit("m", 1)]
        [Conversion(ConversionOperation.Divide, 25.4, ConversionOperation.Multiply, 1000)]
        Meter,
        [Unit("km", 3)]
        [Conversion(ConversionOperation.Divide, 25.4, ConversionOperation.Multiply, 1_000_000)]
        Kilometer,
    }
}
