namespace Gehtsoft.Measurements
{
    /// <summary>
    /// <para>The operation used to specify conversion of one unit to another.</para>
    /// <para>The value is used in <see cref="ConversionAttribute"/></para>
    /// </summary>
    public enum ConversionOperation
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        None,
        /// <summary>
        /// <para>Marks the unit as a base unit.</para>
        /// <para>Cannot be combined with other operations</para>
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
        /// <para>Negates the value</para>
        /// <para>The factor is ignored</para>
        /// </summary>
        Negate,
        /// <summary>
        /// <para>Calculates tangent of the value</para>
        /// <para>The factor is ignored</para>
        /// </summary>
        Tan,
        /// <summary>
        /// <para>Calculates arctangent of the value</para>
        /// <para>The factor is ignored</para>
        /// </summary>
        Atan,
        
        /// <summary>
        /// <para>The custom conversion.</para>
        /// <para>The custom conversion should be implemented using <see cref="ICustomConversionOperation"/> interface.</para>
        /// <para>Cannot be combined with other operations.</para>
        /// </summary>
        Custom,
    };
}
