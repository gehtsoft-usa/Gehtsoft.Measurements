namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units of volume
    /// </summary>
    public enum AreaUnit
    {
        /// <summary>
        /// Square millimeter
        /// </summary>
        [Unit("mm²", "mm2", 0)]
        [Conversion(ConversionOperation.Base)]
        SquareMillimeter = 0,

        /// <summary>
        /// Square centimeter
        /// </summary>
        [Unit("cm²", "cm2", 0)]
        [Conversion(ConversionOperation.Multiply, 100)]
        SquareCentimeter,

        /// <summary>
        /// Square meter
        /// </summary>
        [Unit("m²", "m2", 0)]
        [Conversion(ConversionOperation.Multiply, 1000000)]
        SquareMeter,

        /// <summary>
        /// Square kilometer
        /// </summary>
        [Unit("km²", "km2", 0)]
        [Conversion(ConversionOperation.Multiply, 1e12)]
        SquareKilometer,

        /// <summary>
        /// Square inch
        /// </summary>
        [Unit("in²", "in2", 0)]
        [Conversion(ConversionOperation.Multiply, 645.16)]
        SquareInch,

        /// <summary>
        /// Square foot
        /// </summary>
        [Unit("ft²", "ft2", 0)]
        [Conversion(ConversionOperation.Multiply, 92903.04)]
        SquareFoot,

        /// <summary>
        /// Square yard
        /// </summary>
        [Unit("yd²", "yd2", 0)]
        [Conversion(ConversionOperation.Multiply, 836127.36)]
        SquareYard,

        /// <summary>
        /// Square mile
        /// </summary>
        [Unit("mi²", "mi2", 0)]
        [Conversion(ConversionOperation.Multiply, 2589988110000)]
        SquareMile,

        /// <summary>
        /// Acre
        /// </summary>
        [Unit("ac", 0)]
        [Conversion(ConversionOperation.Multiply, 4046856422.4)]
        Acre,

        /// <summary>
        /// Hectare
        /// </summary>
        [Unit("ha", 0)]
        [Conversion(ConversionOperation.Multiply, 1e+10)]
        Hectare,

        /// <summary>
        /// Ar (1/100 of hectare, "sotka")
        /// </summary>
        [Unit("ar", 0)]
        [Conversion(ConversionOperation.Multiply, 1e+8)]
        Ar,
    }
}