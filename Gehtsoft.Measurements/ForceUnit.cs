namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The units of force
    /// </summary>
    public enum ForceUnit
    {
        /// <summary>
        /// Newton (aka kg·m/s²)
        /// </summary>
        [Unit("N", "kg·m/s²", 3)]
        [Conversion(ConversionOperation.Base)]
        Newton,

        /// <summary>
        /// Dyne
        /// </summary>
        [Unit("dyn", 3)]
        [Conversion(ConversionOperation.Divide, 100000)]
        Dyne,

        /// <summary>
        /// Kilogram-force or kilopond
        /// </summary>
        [Unit("kp", 3)]
        [Conversion(ConversionOperation.Multiply, 9.80665)]
        KilogramForce,

        /// <summary>
        /// Pound-force
        /// </summary>
        [Conversion(ConversionOperation.Multiply, 4.448222)]
        [Unit("lbf", 3)]
        PoundForce,

        /// <summary>
        /// Poundal
        /// </summary>
        [Conversion(ConversionOperation.Multiply, 0.138255)]
        [Unit("pdl", 3)]
        Poundal,
    }
}