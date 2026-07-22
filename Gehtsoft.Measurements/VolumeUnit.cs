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
        [Conversion(ConversionOperation.Multiply, 568.26125)]
        ImperialPint,

        /// <summary>
        /// Imperial quart
        /// </summary>
        [Unit("imp.qt", 1)]
        [Conversion(ConversionOperation.Multiply, 1136.5225)]
        ImperialQuart,

        /// <summary>
        /// Imperial gallon
        /// </summary>
        [Unit("imp.gal", 1)]
        [Conversion(ConversionOperation.Multiply, 4546.09)]
        ImperialGallon,

        /// <summary>
        /// US fluid ounce
        /// </summary>
        [Unit("oz", 1)]
        [Conversion(ConversionOperation.Multiply, 29.5735295625)]
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
        [Conversion(ConversionOperation.Multiply, 3785.411784)]
        Gallon,

        /// <summary>
        /// Cubic centimeter (equal to one milliliter)
        /// </summary>
        [Unit("cc", 1)]
        [Conversion(ConversionOperation.Multiply, 1)]
        CubicCentimeter,

        /// <summary>
        /// Imperial fluid ounce
        /// </summary>
        [Unit("imp.oz", 1)]
        [Conversion(ConversionOperation.Multiply, 28.4130625)]
        ImperialFluidOunce,

        /// <summary>
        /// Oil barrel (42 US gallons)
        /// </summary>
        [Unit("bbl", 1)]
        [Conversion(ConversionOperation.Multiply, 42 * 3785.411784)]
        OilBarrel,

        /// <summary>
        /// US teaspoon
        /// </summary>
        [Unit("tsp", 1)]
        [Conversion(ConversionOperation.Multiply, 4.92892159375)]
        Teaspoon,

        /// <summary>
        /// US tablespoon
        /// </summary>
        [Unit("tbsp", 1)]
        [Conversion(ConversionOperation.Multiply, 14.78676478125)]
        Tablespoon,

        /// <summary>
        /// US customary cup (8 US fluid ounces)
        /// </summary>
        [Unit("cup", 1)]
        [Conversion(ConversionOperation.Multiply, 8 * 29.5735295625)]
        Cup,
    }
}
