namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The distance units
    /// </summary>
    public enum DistanceUnit
    {
        /// <summary>
        /// English line (1/12 inch)
        /// </summary>
        [Unit("ln", "'''", 1)]
        [Conversion(ConversionOperation.Divide, 12)]
        Line,

        /// <summary>
        /// Russian line (1/10 inch)
        /// </summary>
        [Unit("rln", 1)]
        [Conversion(ConversionOperation.Divide, 10)]
        RussianLine,

        /// <summary>
        /// Inch
        /// </summary>
        [Unit("in", "\"", 1)]
        [Conversion(ConversionOperation.Base)]
        Inch,

        /// <summary>
        /// Foot
        /// </summary>
        [Unit("ft", "\'", 2)]
        [Conversion(ConversionOperation.Multiply, 12)]
        Foot,

        /// <summary>
        /// Yard
        /// </summary>
        [Unit("yd", 2)]
        [Conversion(ConversionOperation.Multiply, 36)]
        Yard,

        /// <summary>
        /// Mile
        /// </summary>
        [Unit("mi", 3)]
        [Conversion(ConversionOperation.Multiply, 63360)]
        Mile,

        /// <summary>
        /// Nautical Mile
        /// </summary>
        [Unit("nm", 3)]
        [Conversion(ConversionOperation.Multiply, 1_852_000, ConversionOperation.Divide, 25.4)]
        NauticalMile,

        /// <summary>
        /// Millimeter
        /// </summary>
        [Unit("mm", 0)]
        [Conversion(ConversionOperation.Divide, 25.4)]
        Millimeter,

        /// <summary>
        /// Centimeter
        /// </summary>
        [Unit("cm", 1)]
        [Conversion(ConversionOperation.Divide, 2.54)]
        Centimeter,

        /// <summary>
        /// Meter
        /// </summary>
        [Unit("m", 1)]
        [Conversion(ConversionOperation.Divide, 25.4, ConversionOperation.Multiply, 1000)]
        Meter,

        /// <summary>
        /// Kilometer
        /// </summary>
        [Unit("km", 3)]
        [Conversion(ConversionOperation.Divide, 25.4, ConversionOperation.Multiply, 1_000_000)]
        Kilometer,

        /// <summary>
        /// Typographical/DTP point (1 pt == 1/72 of inch)
        /// </summary>
        [Unit("pt", 1)]
        [Conversion(ConversionOperation.Divide, 72)]
        Point,

        /// <summary>
        /// Typographical/DTP pica (1 pt == 1/6 of inch)
        /// </summary>
        [Unit("p", 1)]
        [Conversion(ConversionOperation.Divide, 6)]
        Pica,

        /// <summary>
        /// Micrometer (micron)
        /// </summary>
        [Unit("µm", 0)]
        [Conversion(ConversionOperation.Divide, 25400)]
        Micrometer,

        /// <summary>
        /// Decimeter
        /// </summary>
        [Unit("dm", 2)]
        [Conversion(ConversionOperation.Divide, 0.254)]
        Decimeter,

        /// <summary>
        /// Thou (1/1000 inch)
        /// </summary>
        [Unit("thou", 0)]
        [Conversion(ConversionOperation.Divide, 1000)]
        Thou,

        /// <summary>
        /// Furlong (660 international feet)
        /// </summary>
        // Based on the international foot (0.3048 m), consistent with Foot/Mile above.
        // Some references use the US survey foot for the furlong (~0.30480061 m).
        [Unit("fur", 3)]
        [Conversion(ConversionOperation.Multiply, 7920)]
        Furlong,

        /// <summary>
        /// Fathom (6 international feet)
        /// </summary>
        [Unit("ftm", 2)]
        [Conversion(ConversionOperation.Multiply, 72)]
        Fathom,

        /// <summary>
        /// Hand (4 inches)
        /// </summary>
        [Unit("hh", 1)]
        [Conversion(ConversionOperation.Multiply, 4)]
        Hand,
    }
}


