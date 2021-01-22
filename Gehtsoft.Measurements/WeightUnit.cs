namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Weight units
    /// </summary>
    public enum WeightUnit : int
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
        /// Newton
        /// </summary>
        [Unit("N", 3)]
        [Conversion(ConversionOperation.Multiply, 1573.6626)]
        Neuton,

        /// <summary>
        /// Dram
        /// </summary>
        [Unit("dr", 1)]
        [Conversion(ConversionOperation.Multiply, 1.7718451953125)]
        Dram
    }


}
