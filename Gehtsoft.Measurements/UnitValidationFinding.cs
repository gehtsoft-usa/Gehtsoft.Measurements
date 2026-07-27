namespace Gehtsoft.Measurements
{
    /// <summary>
    /// One problem found in a unit enumeration.
    /// </summary>
    public sealed class UnitValidationFinding
    {
        /// <summary>
        /// The stable code of the check which produced the finding, such as `"GM007"`.
        /// </summary>
        /// <remarks>
        /// The code never changes for a given check, so it can be used to filter a finding out
        /// or to look the check up. The text of the message may change.
        /// </remarks>
        public string Code { get; }

        /// <summary>
        /// How serious the finding is.
        /// </summary>
        public UnitValidationSeverity Severity { get; }

        /// <summary>
        /// The name of the offending unit, or `null` when the finding is about the enumeration itself.
        /// </summary>
        public string Unit { get; }

        /// <summary>
        /// What is wrong.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="severity"></param>
        /// <param name="unit">The name of the offending unit, or `null` for the enumeration itself</param>
        /// <param name="message"></param>
        public UnitValidationFinding(string code, UnitValidationSeverity severity, string unit, string message)
        {
            Code = code;
            Severity = severity;
            Unit = unit;
            Message = message;
        }

        /// <summary>
        /// Returns the finding as one line of text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Unit == null
                   ? $"{Code} {Severity}: {Message}"
                   : $"{Code} {Severity}: {Unit}: {Message}";
    }
}
