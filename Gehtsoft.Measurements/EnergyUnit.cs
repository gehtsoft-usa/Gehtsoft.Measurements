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
        [Conversion(ConversionOperation.Multiply, 1055)]
        BTU,

        /// <summary>
        /// Hoursepowers-hour
        /// </summary>
        [Unit("hp·h", "hp-h", 0)]
        [Conversion(ConversionOperation.Multiply, 2_684_500)]
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
    }
}
