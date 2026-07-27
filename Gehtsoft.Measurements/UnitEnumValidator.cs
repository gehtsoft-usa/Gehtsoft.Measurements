using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// Checks that a unit enumeration is declared correctly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A unit enumeration is ordinary C# decorated with attributes, so most of the ways to get
    /// it wrong are invisible to the compiler. The measurement types reject the fatal ones when
    /// the enumeration is first used, but they do so one at a time and from a static
    /// initializer. This validator reports everything at once and returns it instead of
    /// throwing, which is what makes it usable while an application starts or from a test.
    /// </para>
    /// <para>
    /// It is meant above all for enumerations declared outside this library. Call it once for
    /// every unit type the application defines.
    /// </para>
    /// </remarks>
    /// <example>
    /// @code
    /// UnitEnumValidator.Validate&lt;MyWeightUnit&gt;().ThrowIfInvalid();
    /// @endcode
    /// </example>
    public static class UnitEnumValidator
    {
        /// <summary>
        /// The values the conversions are exercised with.
        /// </summary>
        /// <remarks>
        /// Positive, negative, small and large, so that a reverse operation which only happens
        /// to work for one of them is still caught.
        /// </remarks>
        private static readonly double[] gProbes = new double[] { 1, 2.5, -3.75, 0.001, 1000 };

        /// <summary>
        /// The values used instead for a conversion which takes a tangent on the way to the base unit.
        /// </summary>
        /// <remarks>
        /// The tangent repeats every half turn, so the arc tangent only undoes it between minus
        /// and plus a quarter turn. Probing such a unit outside that range reports a conversion
        /// as broken when it is merely periodic, so the probes stay inside it.
        /// </remarks>
        private static readonly double[] gTangentProbes = new double[] { 1, 0.25, -0.5, 0.001 };

        /// <summary>
        /// The value used to check that a formatted measurement can be read back.
        /// </summary>
        private const double ParseProbe = 1.25;

        /// <summary>
        /// The relative tolerance of the conversion round-trip.
        /// </summary>
        /// <remarks>
        /// Far looser than the tolerance the comparison operators use, because a conversion
        /// which goes through a trigonometric operation loses more than a multiplication does.
        /// The check is looking for a wrong operation, not for the last few bits.
        /// </remarks>
        private const double RoundTripTolerance = 1e-9;

        /// <summary>
        /// Validates the unit enumeration.
        /// </summary>
        /// <typeparam name="T">The unit enumeration</typeparam>
        /// <returns></returns>
        public static UnitValidationReport Validate<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>()
            where T : Enum
        {
            var findings = new List<UnitValidationFinding>();
            var units = Collect(typeof(T), findings);

            // The conversions may only be exercised once the declaration is known to be sound.
            // Touching Measurement<T> raises the validation built into the static initializer,
            // which would both hide the findings collected here behind an exception and leave
            // the closed type faulted for the rest of the process.
            if (!HasError(findings))
                ValidateBehaviour<T>(units, findings);

            return new UnitValidationReport(typeof(T), findings);
        }

        /// <summary>
        /// Validates the unit enumeration passed as a type.
        /// </summary>
        /// <remarks>
        /// The generic overload is the one to prefer. This one closes it over the type at run
        /// time, which neither trimming nor ahead-of-time compilation can follow. It exists for
        /// a caller which discovers the unit enumerations of an assembly instead of naming them.
        /// </remarks>
        /// <param name="unitType">The unit enumeration</param>
        /// <returns></returns>
        [RequiresDynamicCode("Validating a unit enumeration known only at run time closes a generic method over it, which ahead-of-time compilation cannot generate in advance.")]
        [RequiresUnreferencedCode("Validating a unit enumeration known only at run time reflects over its fields, which trimming can remove.")]
        public static UnitValidationReport Validate(Type unitType)
        {
            if (unitType == null)
                throw new ArgumentNullException(nameof(unitType));

            if (!unitType.IsEnum)
            {
                var findings = new List<UnitValidationFinding>
                {
                    new UnitValidationFinding("GM001", UnitValidationSeverity.Error, null, $"{unitType.Name} is not an enumeration"),
                };
                return new UnitValidationReport(unitType, findings);
            }

            MethodInfo generic = typeof(UnitEnumValidator).GetMethod(nameof(Validate), Type.EmptyTypes)
                                                          .MakeGenericMethod(unitType);
            return (UnitValidationReport)generic.Invoke(null, null);
        }

        /// <summary>
        /// What the reflection pass found out about one unit.
        /// </summary>
        private sealed class UnitInfo
        {
            public string Field;
            public object Value;
            public UnitAttribute Unit;
            public ConversionAttribute Conversion;
            public bool Obsolete;
        }

        private static bool HasError(List<UnitValidationFinding> findings)
        {
            for (int i = 0; i < findings.Count; i++)
            {
                if (findings[i].Severity == UnitValidationSeverity.Error)
                    return true;
            }
            return false;
        }

        private static void Add(List<UnitValidationFinding> findings, string code, UnitValidationSeverity severity, string unit, string message)
            => findings.Add(new UnitValidationFinding(code, severity, unit, message));

        /// <summary>
        /// Runs every check which needs nothing but reflection.
        /// </summary>
        private static List<UnitInfo> Collect(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type type,
            List<UnitValidationFinding> findings)
        {
            var units = new List<UnitInfo>();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            int baseCount = 0;
            string firstBase = null;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                var unit = field.GetCustomAttribute<UnitAttribute>();
                var conversion = field.GetCustomAttribute<ConversionAttribute>();

                if (unit == null)
                    Add(findings, "GM002", UnitValidationSeverity.Error, field.Name, "the unit is not marked with the Unit attribute");
                if (conversion == null)
                    Add(findings, "GM003", UnitValidationSeverity.Error, field.Name, "the unit is not marked with the Conversion attribute");

                if (unit == null || conversion == null)
                    continue;

                var info = new UnitInfo
                {
                    Field = field.Name,
                    Value = field.GetRawConstantValue(),
                    Unit = unit,
                    Conversion = conversion,
                    Obsolete = field.GetCustomAttribute<ObsoleteAttribute>() != null,
                };
                units.Add(info);

                if (conversion.Operation == ConversionOperation.Base)
                {
                    baseCount++;
                    if (firstBase == null)
                        firstBase = field.Name;
                }

                CheckNames(info, findings);
                CheckAccuracy(info, findings);
                CheckFactors(info, findings);
                CheckCustomConversion(info, findings);
            }

            if (units.Count == 0)
                Add(findings, "GM001", UnitValidationSeverity.Error, null, $"{type.Name} declares no unit");
            else if (baseCount == 0)
                Add(findings, "GM004", UnitValidationSeverity.Error, null, "no unit is marked with ConversionOperation.Base, so there is nothing to convert through");
            else if (baseCount > 1)
                Add(findings, "GM005", UnitValidationSeverity.Error, null, $"{baseCount} units are marked with ConversionOperation.Base, the first of them is {firstBase}. Exactly one is allowed");

            CheckDuplicateNames(units, findings);
            return units;
        }

        private static void CheckNames(UnitInfo info, List<UnitValidationFinding> findings)
        {
            CheckName(info, info.Unit.Name, "name", findings);
            if (info.Unit.HasAlternativeName)
                CheckName(info, info.Unit.AlternativeName, "alternative name", findings);
        }

        private static void CheckName(UnitInfo info, string name, string what, List<UnitValidationFinding> findings)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Add(findings, "GM006", UnitValidationSeverity.Error, info.Field, $"the {what} is empty");
                return;
            }

            if (info.Obsolete)
                return;         // an obsolete unit is never parsed, so its name cannot confuse the parser

            if (CanEndANumber(name[0]))
                Add(findings, "GM012", UnitValidationSeverity.Warning, info.Field,
                    $"the {what} '{name}' starts with '{name[0]}', which a number can end with, so the name can swallow the last characters of the value. Check the GM017 findings");
        }

        /// <summary>
        /// Whether a number can end with the character.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The parser takes the longest unit name which ends the text and reads what is left in
        /// front of it as the value. A name which starts with a character the value can end
        /// with may therefore take a character away from the value.
        /// </para>
        /// <para>
        /// The separators of the common cultures are included rather than those of one culture,
        /// because the text may be parsed in any of them. The exponent letter is not: a number
        /// never ends with it, so a name starting with it cannot take anything away.
        /// </para>
        /// </remarks>
        private static bool CanEndANumber(char c)
            => (c >= '0' && c <= '9') || c == '.' || c == ',' || c == ' ';

        private static void CheckAccuracy(UnitInfo info, List<UnitValidationFinding> findings)
        {
            if (info.Unit.DefaultAccuracy < 0 || info.Unit.DefaultAccuracy > 15)
                Add(findings, "GM011", UnitValidationSeverity.Warning, info.Field,
                    $"the default accuracy {info.Unit.DefaultAccuracy} is outside the range 0 to 15 which a numeric format supports");
        }

        private static void CheckFactors(UnitInfo info, List<UnitValidationFinding> findings)
        {
            CheckFactor(info, info.Conversion.Operation, info.Conversion.Factor, "the operation", findings);
            CheckFactor(info, info.Conversion.SecondOperation, info.Conversion.SecondFactor, "the second operation", findings);
        }

        private static void CheckFactor(UnitInfo info, ConversionOperation operation, double factor, string what, List<UnitValidationFinding> findings)
        {
            if (!UsesFactor(operation))
            {
                if (factor != 0)
                    Add(findings, "GM010", UnitValidationSeverity.Warning, info.Field,
                        $"{what} {operation} ignores its factor, but the factor is set to {factor.ToString(CultureInfo.InvariantCulture)}");
                return;
            }

            if (double.IsNaN(factor) || double.IsInfinity(factor))
            {
                Add(findings, "GM008", UnitValidationSeverity.Error, info.Field,
                    $"the factor of {what} {operation} is {factor.ToString(CultureInfo.InvariantCulture)}");
                return;
            }

            if (factor == 0 &&
                (operation == ConversionOperation.Multiply ||
                 operation == ConversionOperation.Divide ||
                 operation == ConversionOperation.DivideFactor))
            {
                Add(findings, "GM009", UnitValidationSeverity.Error, info.Field,
                    $"the factor of {what} {operation} is zero, which cannot be converted back");
            }
        }

        private static bool UsesFactor(ConversionOperation operation)
        {
            switch (operation)
            {
                case ConversionOperation.Add:
                case ConversionOperation.Subtract:
                case ConversionOperation.SubtractFromFactor:
                case ConversionOperation.Multiply:
                case ConversionOperation.Divide:
                case ConversionOperation.DivideFactor:
                    return true;
                default:
                    return false;
            }
        }

        private static void CheckCustomConversion(UnitInfo info, List<UnitValidationFinding> findings)
        {
            if (info.Conversion.Operation != ConversionOperation.Custom)
                return;

            if (info.Conversion.ConversionInterface2 == null)
                Add(findings, "GM013", UnitValidationSeverity.Warning, info.Field,
                    $"the custom conversion {info.Conversion.ConversionInterface.GetType().Name} does not implement ICustomConversionOperation2, so a decimal measurement is converted through a double and loses accuracy");
        }

        private static void CheckDuplicateNames(List<UnitInfo> units, List<UnitValidationFinding> findings)
        {
            // matches what the parse list does: an obsolete unit is excluded, so a renamed
            // misspelling sharing the name of its replacement is not a duplicate
            var seen = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (UnitInfo info in units)
            {
                if (info.Obsolete)
                    continue;

                AddName(info, info.Unit.Name, seen, findings);
                if (info.Unit.HasAlternativeName)
                    AddName(info, info.Unit.AlternativeName, seen, findings);
            }
        }

        private static void AddName(UnitInfo info, string name, Dictionary<string, string> seen, List<UnitValidationFinding> findings)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (seen.TryGetValue(name, out string owner))
                Add(findings, "GM007", UnitValidationSeverity.Error, info.Field, $"the name '{name}' is already used by {owner}");
            else
                seen[name] = info.Field;
        }

        /// <summary>
        /// Runs the checks which need the conversions to actually be performed.
        /// </summary>
        private static void ValidateBehaviour<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(
            List<UnitInfo> units, List<UnitValidationFinding> findings)
            where T : Enum
        {
            T baseUnit = Measurement<T>.BaseUnit;

            foreach (double probe in gProbes)
            {
                if (Measurement<T>.ToBase(probe, baseUnit) != probe || Measurement<T>.FromBase(probe, baseUnit) != probe)
                {
                    Add(findings, "GM014", UnitValidationSeverity.Error, baseUnit.ToString(), "the base unit does not convert to itself unchanged");
                    break;
                }
            }

            foreach (UnitInfo info in units)
            {
                var unit = (T)Enum.ToObject(typeof(T), info.Value);
                CheckRoundTrip<T>(unit, info, findings);

                // an obsolete unit is excluded from parsing on purpose, so it has nothing to round-trip
                if (!info.Obsolete)
                    CheckParseRoundTrip<T>(unit, info, findings);
            }
        }

        private static void CheckRoundTrip<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(
            T unit, UnitInfo info, List<UnitValidationFinding> findings)
            where T : Enum
        {
            foreach (double probe in ProbesFor(info.Conversion))
            {
                double toBase = Measurement<T>.ToBase(probe, unit);
                if (double.IsNaN(toBase) || double.IsInfinity(toBase))
                {
                    Add(findings, "GM016", UnitValidationSeverity.Error, info.Field,
                        $"converting {probe.ToString(CultureInfo.InvariantCulture)} to the base unit gives {toBase.ToString(CultureInfo.InvariantCulture)}");
                    return;
                }

                double back = Measurement<T>.FromBase(toBase, unit);
                if (double.IsNaN(back) || double.IsInfinity(back))
                {
                    Add(findings, "GM016", UnitValidationSeverity.Error, info.Field,
                        $"converting {probe.ToString(CultureInfo.InvariantCulture)} back from the base unit gives {back.ToString(CultureInfo.InvariantCulture)}");
                    return;
                }

                if (Math.Abs(back - probe) > RoundTripTolerance * Math.Max(1, Math.Abs(probe)))
                {
                    Add(findings, "GM015", UnitValidationSeverity.Error, info.Field,
                        $"converting {probe.ToString(CultureInfo.InvariantCulture)} to the base unit and back gives {back.ToString(CultureInfo.InvariantCulture)}. The reverse of the conversion is wrong");
                    return;
                }
            }
        }

        /// <summary>
        /// The probe values suitable for the conversion.
        /// </summary>
        private static double[] ProbesFor(ConversionAttribute conversion)
            => conversion.Operation == ConversionOperation.Tan || conversion.SecondOperation == ConversionOperation.Tan
                   ? gTangentProbes
                   : gProbes;

        private static void CheckParseRoundTrip<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(
            T unit, UnitInfo info, List<UnitValidationFinding> findings)
            where T : Enum
        {
            CheckParseRoundTrip<T>(unit, info, info.Unit.Name, findings);
            if (info.Unit.HasAlternativeName)
                CheckParseRoundTrip<T>(unit, info, info.Unit.AlternativeName, findings);
        }

        private static void CheckParseRoundTrip<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(
            T unit, UnitInfo info, string name, List<UnitValidationFinding> findings)
            where T : Enum
        {
            if (string.IsNullOrWhiteSpace(name))
                return;         // already reported as GM006

            string text = ParseProbe.ToString(CultureInfo.InvariantCulture) + name;

            if (!Measurement<T>.TryParse(CultureInfo.InvariantCulture, text, out Measurement<T> parsed))
            {
                Add(findings, "GM017", UnitValidationSeverity.Error, info.Field, $"'{text}' cannot be parsed back");
                return;
            }

            if (!EqualityComparer<T>.Default.Equals(parsed.Unit, unit))
            {
                Add(findings, "GM017", UnitValidationSeverity.Error, info.Field, $"'{text}' is parsed as {parsed.Unit} instead");
                return;
            }

            if (parsed.Value != ParseProbe)
                Add(findings, "GM017", UnitValidationSeverity.Error, info.Field,
                    $"'{text}' is parsed as the value {parsed.Value.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}
