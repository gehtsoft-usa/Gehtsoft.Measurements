using System;

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
        /// Millibar
        /// </summary>
        [Unit("mbar", 1)]
        [Conversion(ConversionOperation.Multiply, 100)]
        Millibar,

        /// <summary>
        /// Atmosphere
        /// </summary>
        [Unit("atm", 3)]
        [Conversion(ConversionOperation.Multiply, 101325)]
        Atmosphere,

        /// <summary>
        /// Technical atmosphere (misspelled name kept for compatibility, use TechnicalAtmosphere instead)
        /// </summary>
        [Obsolete("Use PressureUnit.TechnicalAtmosphere instead. This misspelled member is excluded from GetUnitNames and parsing but still converts.")]
        [Unit("at", 3)]
        [Conversion(ConversionOperation.Multiply, 98066.5)]
        TechincalAtmosphere,

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
        [Conversion(ConversionOperation.Multiply, 6894.76)]
        PoundsPerSquareInch,

        /// <summary>
        /// mm of water
        /// </summary>
        [Unit("mmH2O", 2)]
        [Conversion(ConversionOperation.Multiply, 9.80665)]
        MillimetersOfWater,

        /// <summary>
        /// Technical atmosphere (one kilogram-force per square centimeter)
        /// </summary>
        [Unit("at", 3)]
        [Conversion(ConversionOperation.Multiply, 98066.5)]
        TechnicalAtmosphere,

        /// <summary>
        /// Hectopascal
        /// </summary>
        [Unit("hPa", 1)]
        [Conversion(ConversionOperation.Multiply, 100)]
        Hectopascal,

        /// <summary>
        /// Megapascal
        /// </summary>
        [Unit("MPa", 3)]
        [Conversion(ConversionOperation.Multiply, 1_000_000)]
        Megapascal,

        /// <summary>
        /// Torr (1/760 of a standard atmosphere)
        /// </summary>
        [Unit("torr", 2)]
        [Conversion(ConversionOperation.Multiply, 101325.0 / 760.0)]
        Torr,

        /// <summary>
        /// Inches of water (at standard gravity)
        /// </summary>
        [Unit("inH2O", 2)]
        [Conversion(ConversionOperation.Multiply, 249.08890833)]
        InchesOfWater,

        /// <summary>
        /// Pounds per square foot
        /// </summary>
        [Unit("psf", "lbf/ft2", 2)]
        [Conversion(ConversionOperation.Multiply, 6894.757293168 / 144)]
        PoundsPerSquareFoot,
    }
}
