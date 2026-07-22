namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Energy units
    /// </summary>
    public enum EnergyUnit
    {
        /// <summary>
        /// Feet-pounds
        /// </summary>
        [Unit("ft·lb", "ft-lb", 0)]
        [Conversion(ConversionOperation.Divide, 0.737562149277)]
        FootPound = 0,

        /// <summary>
        /// Joules
        /// </summary>
        [Unit("J", 0)]
        [Conversion(ConversionOperation.Base)]
        Joule,

        /// <summary>
        /// British Thermal Units
        /// </summary>
        [Unit("BTU", 0)]
        [Conversion(ConversionOperation.Multiply, 1055.05585262)]
        BTU,

        /// <summary>
        /// Hoursepowers-hour
        /// </summary>
        [Unit("hp·h", "hp-h", 0)]
        [Conversion(ConversionOperation.Multiply, 745.699872 * 3600)]
        HpH,

        /// <summary>
        /// Watt-hour
        /// </summary>
        [Unit("w·h", "wh", 0)]
        [Conversion(ConversionOperation.Multiply, 3600)]
        Wh,

        /// <summary>
        /// Kilowatt-hour
        /// </summary>
        [Unit("kw·h", "kwh", 0)]
        [Conversion(ConversionOperation.Multiply, 3600000)]
        kWh,

        /// <summary>
        /// Kilojoule
        /// </summary>
        [Unit("kJ", 0)]
        [Conversion(ConversionOperation.Multiply, 1000)]
        Kilojoule,

        /// <summary>
        /// Thermochemical calorie
        /// </summary>
        [Unit("cal", 0)]
        [Conversion(ConversionOperation.Multiply, 4.184)]
        Calorie,

        /// <summary>
        /// Thermochemical kilocalorie (food calorie)
        /// </summary>
        [Unit("kcal", 0)]
        [Conversion(ConversionOperation.Multiply, 4184)]
        Kilocalorie,

        /// <summary>
        /// Erg
        /// </summary>
        [Unit("erg", 0)]
        [Conversion(ConversionOperation.Divide, 10_000_000)]
        Erg,
    }
}
