namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The units of velocity
    /// </summary>
    public enum VelocityUnit
    {
        /// <summary>
        /// Meters per second
        /// </summary>
        [Unit("m/s", "mps", 0)]
        [Conversion(ConversionOperation.Base)]
        MetersPerSecond = 0,

        /// <summary>
        /// Kilometers per hour
        /// </summary>
        [Unit("km/h", "kmph", 1)]
        [Conversion(ConversionOperation.Divide, 3.6)]
        KilometersPerHour,

        /// <summary>
        /// Feet per second
        /// </summary>
        [Unit("ft/s", "fps", 1)]
        [Conversion(ConversionOperation.Multiply, 0.3048)]
        FeetPerSecond,

        /// <summary>
        /// Miles per hour
        /// </summary>
        [Unit("mi/h", "mph", 1)]
        [Conversion(ConversionOperation.Multiply, 0.44704)]
        MilesPerHour,

        /// <summary>
        /// Knots
        /// </summary>
        [Unit("kt", 1)]
        [Conversion(ConversionOperation.Multiply, 1852.0 / 3600.0)]
        Knot,

        /// <summary>
        /// Inches per second
        /// </summary>
        [Unit("in/s", 1)]
        [Conversion(ConversionOperation.Multiply, 0.0254)]
        InchesPerSecond,

        /// <summary>
        /// Centimeters per second
        /// </summary>
        [Unit("cm/s", 1)]
        [Conversion(ConversionOperation.Multiply, 0.01)]
        CentimetersPerSecond,

        /// <summary>
        /// Feet per minute
        /// </summary>
        [Unit("ft/min", 1)]
        [Conversion(ConversionOperation.Multiply, 0.3048 / 60)]
        FeetPerMinute,
    }
}
