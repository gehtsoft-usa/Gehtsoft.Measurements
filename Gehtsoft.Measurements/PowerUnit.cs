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
        /// Metric horse power
        /// </summary>
        [Unit("ps", 1)]
        [Conversion(ConversionOperation.Multiply, 735.5)]
        MetricHoursePower,

        /// <summary>
        /// Imperial/Mechanical horse power
        /// </summary>
        [Unit("hp", 1)]
        [Conversion(ConversionOperation.Multiply, 745.7)]
        MechanicalHoursePower,

        /// <summary>
        /// Foot-pound force
        /// </summary>
        [Unit("ft⋅lbf", "ft-lbf", 1)]
        [Conversion(ConversionOperation.Multiply, 1.3558179483314)]
        FootPound,
    }
}
