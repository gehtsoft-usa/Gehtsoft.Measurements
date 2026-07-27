using System;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units of density
    /// </summary>
    public enum DensityUnit
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
        /// Ounces per cubic inch (misnamed member kept for compatibility, use OuncesPerCubicInch instead)
        /// </summary>
        [Obsolete("Use DensityUnit.OuncesPerCubicInch instead. This member is named after the cubic foot but has always been ounces per cubic inch, which is what its unit name and its factor say. It is excluded from GetUnitNames and parsing but still converts.")]
        [Unit("oz/in³", "oz/in3", 0)]
        [Conversion(ConversionOperation.Multiply, 1729.994)]
        OuncesPerCubicFeet,

        /// <summary>
        /// Pounds per cubic foot
        /// </summary>
        [Unit("lb/ft³", "lb/ft3", 2)]
        [Conversion(ConversionOperation.Multiply, 16.0185)]
        PoundsPerCubicFoot,

        /// <summary>
        /// Grains per cubic inch (powder loading density)
        /// </summary>
        [Unit("gr/in³", "gr/in3", 3)]
        [Conversion(ConversionOperation.Multiply, 64.79891 / 16.387064)]
        GrainsPerCubicInch,

        /// <summary>
        /// Kilograms per liter
        /// </summary>
        [Unit("kg/l", 3)]
        [Conversion(ConversionOperation.Multiply, 1000)]
        KilogramPerLiter,

        /// <summary>
        /// Pounds per US gallon
        /// </summary>
        [Unit("lb/gal", 2)]
        [Conversion(ConversionOperation.Multiply, 453.59237 / 3.785411784)]
        PoundsPerGallon,

        /// <summary>
        /// Ounces per cubic inch
        /// </summary>
        /// <remarks>
        /// Added at the end rather than in place of the misnamed OuncesPerCubicFeet, so that the
        /// numeric value of every other member stays what it was.
        /// </remarks>
        [Unit("oz/in³", "oz/in3", 0)]
        [Conversion(ConversionOperation.Multiply, 1729.994)]
        OuncesPerCubicInch,
    }
}
