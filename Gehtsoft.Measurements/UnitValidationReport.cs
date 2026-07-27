using System;
using System.Collections.Generic;
using System.Text;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The result of validating one unit enumeration.
    /// </summary>
    /// <remarks>
    /// Produced by <see cref="UnitEnumValidator"/>. The report is returned rather than thrown,
    /// so that every problem of the enumeration can be seen at once. Call
    /// `ThrowIfInvalid` when the caller only wants to refuse to start.
    /// </remarks>
    public sealed class UnitValidationReport
    {
        /// <summary>
        /// The enumeration which was validated.
        /// </summary>
        public Type UnitType { get; }

        /// <summary>
        /// Everything which was found, errors and warnings alike, in declaration order.
        /// </summary>
        public IReadOnlyList<UnitValidationFinding> Findings { get; }

        /// <summary>
        /// Whether anything makes the enumeration unusable.
        /// </summary>
        public bool HasErrors { get; }

        /// <summary>
        /// Whether the enumeration is free of errors.
        /// </summary>
        /// <remarks>
        /// Warnings do not make an enumeration invalid.
        /// </remarks>
        public bool IsValid => !HasErrors;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitType"></param>
        /// <param name="findings"></param>
        public UnitValidationReport(Type unitType, IReadOnlyList<UnitValidationFinding> findings)
        {
            UnitType = unitType ?? throw new ArgumentNullException(nameof(unitType));
            Findings = findings ?? throw new ArgumentNullException(nameof(findings));

            for (int i = 0; i < findings.Count; i++)
            {
                if (findings[i].Severity == UnitValidationSeverity.Error)
                {
                    HasErrors = true;
                    break;
                }
            }
        }

        /// <summary>
        /// The findings which make the enumeration unusable.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UnitValidationFinding> Errors() => FindingsOf(UnitValidationSeverity.Error);

        /// <summary>
        /// The findings which are suspicious but not fatal.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UnitValidationFinding> Warnings() => FindingsOf(UnitValidationSeverity.Warning);

        private IEnumerable<UnitValidationFinding> FindingsOf(UnitValidationSeverity severity)
        {
            for (int i = 0; i < Findings.Count; i++)
            {
                if (Findings[i].Severity == severity)
                    yield return Findings[i];
            }
        }

        /// <summary>
        /// Throws when the enumeration has errors.
        /// </summary>
        /// <remarks>
        /// Intended for an application which checks its units while starting up. The message of
        /// the exception lists every finding.
        /// </remarks>
        public void ThrowIfInvalid()
        {
            if (HasErrors)
                throw new InvalidOperationException(ToString());
        }

        /// <summary>
        /// Returns the whole report as text, one finding per line.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Findings.Count == 0)
                return $"{UnitType.Name}: valid";

            var builder = new StringBuilder();
            builder.Append(UnitType.Name).Append(':');
            for (int i = 0; i < Findings.Count; i++)
                builder.AppendLine().Append("  ").Append(Findings[i].ToString());
            return builder.ToString();
        }
    }
}
