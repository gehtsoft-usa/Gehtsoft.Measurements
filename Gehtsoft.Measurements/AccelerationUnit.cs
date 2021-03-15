namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Acceleration
    /// </summary>
    public enum AccelerationUnit
    {
        /// <summary>
        /// Gals or cm/s²
        /// </summary>
        [Unit("gal", "cm/s²", 6)]
        [Conversion(ConversionOperation.Base)]
        Gal,

        /// <summary>
        /// Feet per second in second
        /// </summary>
        [Unit("ft/s²", "ft/s2", 3)]
        [Conversion(ConversionOperation.Multiply, 30.48)]
        FeetPerSecondSquare,

        /// <summary>
        /// Meters per second in second
        /// </summary>
        [Unit("m/s²", "m/s2", 3)]
        [Conversion(ConversionOperation.Multiply, 100)]
        MeterPerSecondSquare,

        /// <summary>
        /// Earth gravity
        /// </summary>
        [Unit("g0", 3)]
        [Conversion(ConversionOperation.Multiply, 980.665)]
        EarthGravity,
    }
}