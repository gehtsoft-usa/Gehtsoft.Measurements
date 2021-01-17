namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The units of velocity
    /// </summary>
    public enum VelocityUnit : int
    {
        /// <summary>
        /// Meters per second
        /// </summary>
        [Unit("m/s", 0)]
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
        [Unit("ft/s", 1)]
        [Conversion(ConversionOperation.Divide, 3.2808399)]
        FeetPerSecond,
        
        /// <summary>
        /// Miles per hour
        /// </summary>
        [Unit("mi/h", "mph", 1)]
        [Conversion(ConversionOperation.Divide, 2.23693629)]
        MilesPerHour,
        
        /// <summary>
        /// Knots
        /// </summary>
        [Unit("kt", 1)]
        [Conversion(ConversionOperation.Divide, 1.94384449)]
        Knot,
    }
}
