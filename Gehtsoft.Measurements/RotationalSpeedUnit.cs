namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units of rotational (angular) speed
    /// </summary>
    public enum RotationalSpeedUnit
    {
        /// <summary>
        /// Radians per second
        /// </summary>
        [Unit("rad/s", 3)]
        [Conversion(ConversionOperation.Base)]
        RadianPerSecond = 0,

        /// <summary>
        /// Revolutions per minute
        /// </summary>
        [Unit("rpm", 1)]
        [Conversion(ConversionOperation.Multiply, 6.28318530717958 / 60)]
        RevolutionsPerMinute,

        /// <summary>
        /// Hertz (revolutions per second)
        /// </summary>
        [Unit("Hz", 3)]
        [Conversion(ConversionOperation.Multiply, 6.28318530717958)]
        Hertz,
    }
}
