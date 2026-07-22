using System;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units of power
    /// </summary>
    public enum PowerUnit
    {
        /// <summary>
        /// Watt
        /// </summary>
        [Unit("w", 1)]
        [Conversion(ConversionOperation.Base)]
        Watt,

        /// <summary>
        /// Metric horse power (misspelled name kept for compatibility, use MetricHorsePower instead)
        /// </summary>
        [Obsolete("Use PowerUnit.MetricHorsePower instead. This misspelled member is excluded from GetUnitNames and parsing but still converts.")]
        [Unit("ps", 1)]
        [Conversion(ConversionOperation.Multiply, 735.49875)]
        MetricHoursePower,

        /// <summary>
        /// Imperial/Mechanical horse power (misspelled name kept for compatibility, use MechanicalHorsePower instead)
        /// </summary>
        [Obsolete("Use PowerUnit.MechanicalHorsePower instead. This misspelled member is excluded from GetUnitNames and parsing but still converts.")]
        [Unit("hp", 1)]
        [Conversion(ConversionOperation.Multiply, 745.699872)]
        MechanicalHoursePower,

        /// <summary>
        /// Foot-pound force
        /// </summary>
        [Unit("ft⋅lbf", "ft-lbf", 1)]
        [Conversion(ConversionOperation.Multiply, 1.3558179483314)]
        FootPound,

        /// <summary>
        /// Metric horse power
        /// </summary>
        [Unit("ps", 1)]
        [Conversion(ConversionOperation.Multiply, 735.49875)]
        MetricHorsePower,

        /// <summary>
        /// Imperial/Mechanical horse power
        /// </summary>
        [Unit("hp", 1)]
        [Conversion(ConversionOperation.Multiply, 745.699872)]
        MechanicalHorsePower,

        /// <summary>
        /// Kilowatt
        /// </summary>
        [Unit("kw", 1)]
        [Conversion(ConversionOperation.Multiply, 1000)]
        Kilowatt,

        /// <summary>
        /// Megawatt
        /// </summary>
        [Unit("Mw", 3)]
        [Conversion(ConversionOperation.Multiply, 1_000_000)]
        Megawatt,

        /// <summary>
        /// British Thermal Units per hour
        /// </summary>
        [Unit("BTU/h", 1)]
        [Conversion(ConversionOperation.Multiply, 0.29307107)]
        BTUPerHour,
    }
}
