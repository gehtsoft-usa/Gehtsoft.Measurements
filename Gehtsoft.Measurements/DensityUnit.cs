namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units of density
    /// </summary>
    public enum DensityUnit : int
    {
        /// <summary>
        /// Gram per cubic centimeters
        /// </summary>
        [Unit("g/cm³", "g/cm3", 0)]
        [Conversion(ConversionOperation.Multiply, 1000)]
        GramPerCubicCentimeter,

        /// <summary>
        /// Kilogram per cubic centimeters
        /// </summary>
        [Unit("kg/m³", "kg/m3", 3)]
        [Conversion(ConversionOperation.Base)]
        KilogramPerCubicMeter,

        /// <summary>
        /// Pounds per cubic inch
        /// </summary>
        [Unit("lb/in³", "lb/in3", 0)]
        [Conversion(ConversionOperation.Multiply, 27679.9)]
        PoundsPerCubicInch,

        /// <summary>
        /// Ounces per cubic inch
        /// </summary>
        [Unit("oz/in³", "oz/in3", 0)]
        [Conversion(ConversionOperation.Multiply, 1729.994)]
        OuncesPerCubicFeet,

        /// <summary>
        /// Pounds per cubic foot
        /// </summary>
        [Unit("lb/ft³", "lb/ft3", 2)]
        [Conversion(ConversionOperation.Multiply, 16.0185)]
        PoundsPerCubicFoot,
    }
}
