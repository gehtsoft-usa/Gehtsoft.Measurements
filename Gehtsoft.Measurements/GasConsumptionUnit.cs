namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Units of gas consumption
    /// </summary>
    public enum GasConsumptionUnit : int
    {
        /// <summary>
        /// Liters per km
        /// </summary>
        [Unit("l/km", 5)]
        [Conversion(ConversionOperation.Base)]
        LiterPerKm,

        /// <summary>
        /// Liters per 100km
        /// </summary>
        [Unit("l/100km", 1)]
        [Conversion(ConversionOperation.Divide, 100)]
        LiterPer100Km,

        /// <summary>
        /// Miles per gallon
        /// </summary>
        [Unit("mpg", "mi/gal", 1)]
        [Conversion(ConversionOperation.DivideFactor, 2.35214583)]
        MilesPerGallon,
    }
}
