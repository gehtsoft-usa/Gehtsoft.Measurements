namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units of volume
    /// </summary>
    public enum VolumeUnit
    {
        /// <summary>
        /// Milliliter
        /// </summary>
        [Unit("ml", 1)]
        [Conversion(ConversionOperation.Base)]
        Milliliter = 0,

        /// <summary>
        /// Liter
        /// </summary>
        [Unit("l", 3)]
        [Conversion(ConversionOperation.Multiply, 1000)]
        Liter,

        /// <summary>
        /// Cubic Meter
        /// </summary>
        [Unit("m³", "m3", 6)]
        [Conversion(ConversionOperation.Multiply, 1_000_000)]
        CubicMeter,

        /// <summary>
        /// Cubic Inch
        /// </summary>
        [Unit("in³", "in3", 6)]
        [Conversion(ConversionOperation.Multiply, 16.38706)]
        CubicInch,

        /// <summary>
        /// Cubic Feet
        /// </summary>
        [Unit("ft³", "ft3", 6)]
        [Conversion(ConversionOperation.Multiply, 28316.83968)]
        CubicFeet,

        /// <summary>
        /// Cubic Yard
        /// </summary>
        [Unit("yd³", "yd3", 6)]
        [Conversion(ConversionOperation.Multiply, 764554.9)]
        CubicYard,

        /// <summary>
        /// Imperial pint
        /// </summary>
        [Unit("imp.pt", 1)]
        [Conversion(ConversionOperation.Multiply, 568)]
        ImperialPint,

        /// <summary>
        /// Imperial quart
        /// </summary>
        [Unit("imp.qt", 1)]
        [Conversion(ConversionOperation.Multiply, 1137)]
        ImperialQuart,

        /// <summary>
        /// Imperial gallon
        /// </summary>
        [Unit("imp.gal", 1)]
        [Conversion(ConversionOperation.Multiply, 4546)]
        ImperialGallon,

        /// <summary>
        /// US fluid pint
        /// </summary>
        [Unit("oz", 1)]
        [Conversion(ConversionOperation.Multiply, 29.57)]
        Ounce,

        /// <summary>
        /// US liquid pint
        /// </summary>
        [Unit("pt", 1)]
        [Conversion(ConversionOperation.Multiply, 473.176473)]
        Pint,

        /// <summary>
        /// US liquid quart
        /// </summary>
        [Unit("qt", 1)]
        [Conversion(ConversionOperation.Multiply, 946.3529)]
        Quart,

        /// <summary>
        /// US liquid gallon
        /// </summary>
        [Unit("gal", 1)]
        [Conversion(ConversionOperation.Multiply, 3785.412)]
        Gallon,
    }
}