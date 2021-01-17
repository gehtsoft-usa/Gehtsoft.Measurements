namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The operation used to specify conversion of one unit to another.
    /// 
    /// The value is used in <see cref="ConversionAttribute"/>
    /// </summary>
    public enum ConversionOperation
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        None,
        /// <summary>
        /// Marks the unit as a base unit. 
        /// 
        /// Cannot be combined with other operations
        /// </summary>
        Base,
        /// <summary>
        /// Adds the factor to the value
        /// </summary>
        Add,
        /// <summary>
        /// Subtracts factor from the value
        /// </summary>
        Subtract,
        /// <summary>
        /// Subtracts value from the factor
        /// </summary>
        SubtractFromFactor,
        /// <summary>
        /// Multiples the value by the factor
        /// </summary>
        Multiply,
        /// <summary>
        /// Divides the value by the factor
        /// </summary>
        Divide,
        /// <summary>
        /// Divides the factor by the value
        /// </summary>
        DivideFactor,
        /// <summary>
        /// Negates the value
        /// 
        /// The factor is ignored
        /// </summary>
        Negate,
        /// <summary>
        /// Calculates arctangent of the value
        /// 
        /// The factor is ignored
        /// </summary>
        Atan,
        /// <summary>
        /// The custom conversion. 
        /// 
        /// The custom conversion should be implemented using <see cref="ICustomConversionOperation"/> interface. 
        /// 
        /// Cannot be combined with other operations. 
        /// </summary>
        Custom,
    };
}
