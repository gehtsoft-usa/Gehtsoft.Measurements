namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Temperature units
    /// </summary>
    public enum TemperatureUnit
    {
        /// <summary>
        /// Degrees of Fahrenheit
        /// </summary>
        [Unit("°F", "F", 1)]
        [Conversion(ConversionOperation.Base)]
        Fahrenheit = 0,

        /// <summary>
        /// Degrees of Celsius
        /// </summary>
        [Unit("°C", "C", 1)]
        [Conversion(ConversionOperation.Multiply, 1.8, ConversionOperation.Add, 32)]
        Celsius,

        /// <summary>
        /// Degrees of Kelvin
        /// </summary>
        [Unit("°K", "K", 1)]
        [Conversion(ConversionOperation.Multiply, 1.8, ConversionOperation.Subtract, 459.67)]
        Kelvin,

        /// <summary>
        /// Rankin scale
        /// </summary>
        [Unit("°R", "R", 1)]
        [Conversion(ConversionOperation.Subtract, 459.67)]
        Rankin,

        /// <summary>
        /// Reaumur scale
        /// </summary>
        [Unit("°Re", "Re", 1)]
        [Conversion(ConversionOperation.Multiply, 2.25, ConversionOperation.Add, 32)]
        Reaumur,

        /// <summary>
        /// Delisle scale
        /// </summary>
        [Unit("°De", "De", 1)]
        [Conversion(ConversionOperation.Multiply, -1.2, ConversionOperation.Add, 212)]
        Delisle,
    }
}
