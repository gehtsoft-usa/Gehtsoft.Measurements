namespace Gehtsoft.Measurements
{
    /// <summary>
    /// How serious a finding of the unit enumeration validation is.
    /// </summary>
    public enum UnitValidationSeverity
    {
        /// <summary>
        /// <para>The declaration is legal but suspicious.</para>
        /// <para>
        /// The measurements work, but the unit is likely not to behave the way its author
        /// intended. A factor on an operation which ignores it, or a unit name which the parser
        /// can split in the wrong place, are warnings.
        /// </para>
        /// </summary>
        Warning,

        /// <summary>
        /// <para>The declaration is wrong.</para>
        /// <para>
        /// The unit enumeration either cannot be used at all, or produces wrong numbers. A
        /// missing base unit, two units sharing a name, or a conversion which does not
        /// round-trip, are errors.
        /// </para>
        /// </summary>
        Error,
    }
}
