namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The interface to implement the custom conversion
    /// </summary>
    public interface ICustomConversionOperation2 : ICustomConversionOperation
    {
        /// <summary>
        /// Converts the unit to base unit
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        decimal ToBaseDecimal(decimal value);

        /// <summary>
        /// Converts to unit from the base unit
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        decimal FromBaseDecimal(decimal value);
    };
}
