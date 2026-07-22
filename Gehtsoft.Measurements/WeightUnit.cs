using System;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Weight units
    /// </summary>
    public enum WeightUnit
    {
        /// <summary>
        /// Grains
        /// </summary>
        [Unit("gr", 0)]
        [Conversion(ConversionOperation.Base)]
        Grain = 0,

        /// <summary>
        /// Ounces
        /// </summary>
        [Unit("oz", 1)]
        [Conversion(ConversionOperation.Multiply, 437.5)]
        Ounce,

        /// <summary>
        /// Grams
        /// </summary>
        [Unit("g", 1)]
        [Conversion(ConversionOperation.Multiply, 15.4323583529)]
        Gram,

        /// <summary>
        /// Points
        /// </summary>
        [Unit("lb", 3)]
        [Conversion(ConversionOperation.Multiply, 7000)]
        Pound,

        /// <summary>
        /// Kilograms
        /// </summary>
        [Unit("kg", 3)]
        [Conversion(ConversionOperation.Multiply, 15432.3583529)]
        Kilogram,

        /// <summary>
        /// Newton (misspelled name kept for compatibility, use Newton instead)
        /// </summary>
        [Obsolete("Use WeightUnit.Newton instead. This misspelled member is excluded from GetUnitNames and parsing but still converts.")]
        [Unit("N", 3)]
        [Conversion(ConversionOperation.Multiply, 1573.6626)]
        Neuton,

        /// <summary>
        /// Dram
        /// </summary>
        [Unit("dr", 1)]
        [Conversion(ConversionOperation.Multiply, 1.7718451953125)]
        Dram,

        /// <summary>
        /// Troy Ounce
        /// </summary>
        [Unit("tr.oz", 1)]
        [Conversion(ConversionOperation.Multiply, 15.4323583529 * 31.1034768)]
        TroyOz,

        /// <summary>
        /// Metric Tonne
        /// </summary>
        [Unit("t", 3)]
        [Conversion(ConversionOperation.Multiply, 15432358.3529)]
        Tonne,

        /// <summary>
        /// US Tonne (2000 pounds)
        /// </summary>
        [Unit("us.t", 3)]
        [Conversion(ConversionOperation.Multiply, 14_000_000)]
        USTonne,

        /// <summary>
        /// UK Tonne (2240 pounds)
        /// </summary>
        [Unit("uk.t", 3)]
        [Conversion(ConversionOperation.Multiply, 15_680_000)]
        UKTonne,

        /// <summary>
        /// Newton (weight expressed as its mass-equivalent at standard gravity)
        /// </summary>
        [Unit("N", 3)]
        [Conversion(ConversionOperation.Multiply, 1573.6626)]
        Newton,

        /// <summary>
        /// Milligram
        /// </summary>
        [Unit("mg", 0)]
        [Conversion(ConversionOperation.Multiply, 15.4323583529 / 1000)]
        Milligram,

        /// <summary>
        /// Stone (14 pounds)
        /// </summary>
        [Unit("st", 2)]
        [Conversion(ConversionOperation.Multiply, 14 * 7000)]
        Stone,

        /// <summary>
        /// Metric carat (0.2 gram)
        /// </summary>
        [Unit("ct", 2)]
        [Conversion(ConversionOperation.Multiply, 0.2 * 15.4323583529)]
        Carat,

        /// <summary>
        /// Slug (mass accelerated one foot per second squared by one pound-force)
        /// </summary>
        [Unit("slug", 3)]
        [Conversion(ConversionOperation.Multiply, 14.5939029372 * 15432.3583529)]
        Slug,
    }
}
