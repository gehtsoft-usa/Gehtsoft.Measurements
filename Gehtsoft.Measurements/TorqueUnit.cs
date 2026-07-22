namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units of torque (moment of force)
    /// </summary>
    public enum TorqueUnit
    {
        /// <summary>
        /// Newton-meter
        /// </summary>
        [Unit("N·m", "N-m", 3)]
        [Conversion(ConversionOperation.Base)]
        NewtonMeter = 0,

        /// <summary>
        /// Kilogram-force meter
        /// </summary>
        [Unit("kgf·m", "kgf-m", 3)]
        [Conversion(ConversionOperation.Multiply, 9.80665)]
        KilogramForceMeter,

        /// <summary>
        /// Foot-pound force
        /// </summary>
        [Unit("ft·lbf", "ft-lbf", 3)]
        [Conversion(ConversionOperation.Multiply, 1.3558179483314)]
        FootPoundForce,

        /// <summary>
        /// Inch-pound force
        /// </summary>
        [Unit("in·lbf", "in-lbf", 3)]
        [Conversion(ConversionOperation.Multiply, 1.3558179483314 / 12)]
        InchPoundForce,
    }
}
