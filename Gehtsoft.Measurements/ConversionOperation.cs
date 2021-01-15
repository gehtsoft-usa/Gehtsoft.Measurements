namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The operation used to specify conversion of one unit to another.
    /// 
    /// The value is used in <see cref="ConversionAttribute"/>
    /// </summary>
    public enum ConversionOperation
    {
        None,
        Base,
        Add,
        Subtract,
        SubtractFromFactor,
        Multiply,
        Divide,
        DivideFactor,
        Negate,
        Atan,
        Custom,
    };
}
