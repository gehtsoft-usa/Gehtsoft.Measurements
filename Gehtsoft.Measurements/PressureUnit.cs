namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units of pressure
    /// </summary>
    public enum PressureUnit
    {
        /// <summary>
        /// Pascal
        /// </summary>
        [Unit("pa", 0)]
        [Conversion(ConversionOperation.Base)]
        Pascal,

        /// <summary>
        /// Kilo Pascal
        /// </summary>
        [Unit("kPa", 1)]
        [Conversion(ConversionOperation.Multiply, 1000)]
        KiloPascal,

        /// <summary>
        /// Bar
        /// </summary>
        [Unit("bar", 3)]
        [Conversion(ConversionOperation.Multiply, 100000)]
        Bar,

        /// <summary>
        /// Atmosphere
        /// </summary>
        [Unit("atm", 3)]
        [Conversion(ConversionOperation.Multiply, 101325)]
        Atmosphere,

        /// <summary>
        /// Millimeters of mercury
        /// </summary>
        [Unit("mmHg", 1)]
        [Conversion(ConversionOperation.Multiply, 133.322387415)]
        MillimetersOfMercury,

        /// <summary>
        /// Inches of mercury
        /// </summary>
        [Unit("inHg", 2)]
        [Conversion(ConversionOperation.Multiply, 3386.389)]
        InchesOfMercury,

        /// <summary>
        /// Pounds per square inch
        /// </summary>
        [Unit("psi", "lbf/in2", 1)]
        [Conversion(ConversionOperation.Multiply, 6895)]
        PountsPerSquareInch,
    }
}
