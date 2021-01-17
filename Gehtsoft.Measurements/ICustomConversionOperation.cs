namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The interface to implement the custom conversion
    /// </summary>
    public interface ICustomConversionOperation
    {
        /// <summary>
        /// Converts the unit to base unit
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        double ToBase(double value);

        /// <summary>
        /// Converts to unit from the base unit
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        double FromBase(double value);
    };
}
