using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// <para>The class to manipulate measurements expressed in the specified units.</para>
    /// <para>
    /// The unit of the measurement (e.g. length, weight) is defined as the parameter of this generic structure.
    /// The enumeration used as a measurement units specification must be marked using <see cref="UnitAttribute"/> and
    /// <see cref="ConversionAttribute"/>
    /// </para>
    /// <para>The arithmetic operators (e.g. `+`, `*`) and comparison operators are supported.</para>
    /// <para>
    /// The class supports serialization using `System.Text.Json` serializer and `XmlSerializer` as well as
    /// many 3rd party serializers such as `BinaronSerializer`.
    /// </para>
    /// </summary>
    public readonly struct Measurement<T> : IEquatable<Measurement<T>>, IComparable<Measurement<T>>, IFormattable
        where T : Enum
    {
        /// <summary>
        /// Numerical value
        /// </summary>
        [JsonIgnore]
        public readonly double Value;

        /// <summary>
        /// The unit
        /// </summary>
        [JsonIgnore]
        public readonly T Unit;

        /// <summary>
        /// Constructor that accepts numeric value and unit
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Measurement(double value, T unit)
        {
            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Constructor that accepts a tuple.
        /// </summary>
        /// <param name="value"></param>
        public Measurement(Tuple<double, T> value)
        {
            Value = value.Item1;
            Unit = value.Item2;
        }

        /// <summary>
        /// Constructor that accepts a anonymous tuple.
        /// </summary>
        /// <param name="value"></param>
        public Measurement((double, T) value)
        {
            Value = value.Item1;
            Unit = value.Item2;
        }

        /// <summary>
        /// Constructor that accepts a text representation of a value
        /// </summary>
        /// <param name="text"></param>
        [JsonConstructor]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Measurement(string text)
        {
            if (!Measurement<T>.TryParseInternal(CultureInfo.InvariantCulture, text, out double value, out T unit))
                throw new ArgumentException("Invalid value", nameof(text));

            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// The value as a string with maximum accuracy in invariant culture
        /// </summary>
        [JsonPropertyName("value")]
        public string Text => ToString("NF", CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert to string with maximum accuracy in invariant culture
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert to string with maximum accuracy in the specified culture
        /// </summary>
        public string ToString(IFormatProvider cultureInfo) => ToString("NF", cultureInfo);

        /// <summary>
        /// Convert to string with specified format
        /// </summary>
        /// <param name="format">A numeric format or `"ND"` to format with the default accuracy and `"NF"` to display as all digits after decimal point</param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "ND")
                format = $"N{GetUnitDefaultAccuracy(Unit)}";
            return $"{(format == "NF" ? Value.ToString(formatProvider) : Value.ToString(format, formatProvider))}{GetUnitName(Unit)}";
        }

        /// <summary>
        /// Returns the value in the specified units
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double In(T unit) => Convert(Value, Unit, unit);

        /// <summary>
        /// Converts the value into another unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Measurement<T> To(T unit) => new Measurement<T>(In(unit), unit);

        /// <summary>
        /// Convert value from one unit to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Convert(double value, T from, T to)
        {
            // EqualityComparer<T>.Default devirtualizes to an unboxed integer compare
            // for enums on .NET Core+, unlike Enum.CompareTo(object) which boxes both operands.
            if (EqualityComparer<T>.Default.Equals(from, to))
                return value;
            // ToBase/FromBase already return the value unchanged for the base unit,
            // so the explicit base-unit checks are redundant.
            return FromBase(ToBase(value, from), to);
        }

        /// <summary>
        /// The base unit for the measurement
        /// </summary>
        public static T BaseUnit { get; } = UnitUtils.GetBase<T>();

        /// <summary>
        /// The value with a zero measurement
        /// </summary>
        public static Measurement<T> ZERO { get; } = new Measurement<T>(0, UnitUtils.GetBase<T>());

        private static readonly Func<T, string> mGetUnitName = CodeGenerator.GenerateGetUnitName<T>();
        private static readonly (string Name, T Unit)[] mParseList = UnitUtils.GetParseList<T>();
        private static readonly Tuple<T, string>[] mUnitNames = UnitUtils.GetUnits<T>();
        private static readonly Func<T, int> mDefaultAccuracy = CodeGenerator.GenerateGetDefaultUnitAccuracy<T>();
        private static readonly Func<double, T, double> mToBase = CodeGenerator.GenerateConversion<T>(true);
        private static readonly Func<double, T, double> mFromBase = CodeGenerator.GenerateConversion<T>(false);

        /// <summary>
        /// Converts the value from the specified units to a base unit.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToBase(double value, T unit) => mToBase(value, unit);

        /// <summary>
        /// Converts the value to the specified unit  from a base unit.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double FromBase(double value, T unit) => mFromBase(value, unit);

        /// <summary>
        /// Returns all units with their names
        /// </summary>
        /// <returns></returns>
        public static Tuple<T, string>[] GetUnitNames() => (Tuple<T, string>[])mUnitNames.Clone();

        /// <summary>
        /// Gets the name of the unit by its code
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string GetUnitName(T unit) => mGetUnitName(unit);

        /// <summary>
        /// Gets the default accuracy of for the specified unit
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetUnitDefaultAccuracy(T unit) => mDefaultAccuracy(unit);

        /// <summary>
        /// Parses the unit name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T ParseUnitName(string name)
        {
            if (TryParseUnit(name.AsSpan(), out T unit))
                return unit;
            throw new ArgumentException("Unknown unit", nameof(name));
        }

        // Zero-allocation, non-throwing unit-name lookup. Ordinal span comparison over
        // the small unit set lets the parser avoid allocating substrings entirely.
        private static bool TryParseUnit(ReadOnlySpan<char> name, out T unit)
        {
            var list = mParseList;
            for (int i = 0; i < list.Length; i++)
            {
                if (name.SequenceEqual(list[i].Name.AsSpan()))
                {
                    unit = list[i].Unit;
                    return true;
                }
            }
            unit = default;
            return false;
        }

        /// <summary>
        /// Try to parse the value using the current culture
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(string text, out Measurement<T> value) => TryParse(CultureInfo.CurrentCulture, text, out value);

        /// <summary>
        /// Try to parse the value using the specified culture
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(CultureInfo cultureInfo, string text, out Measurement<T> value)
        {
            bool rc = TryParseInternal(cultureInfo, text, out double _value, out T unit);
            if (rc)
                value = new Measurement<T>(_value, unit);
            else
                value = new Measurement<T>(0, default);
            return rc;
        }

        /// <summary>
        /// Returns hash code of the value
        /// </summary>
        /// <returns></returns>
        override public int GetHashCode()
        {
            // The hash must agree with the tolerance-based Equals: values that compare
            // equal must hash equal. The exact base values of two tolerance-equal
            // measurements differ by conversion rounding, so we quantize to fewer
            // significant digits than the comparison tolerance (1e-12 relative) before
            // hashing. A rare boundary straddle can still hash differently; that only
            // degrades a hashed-collection lookup, it never corrupts the collection.
            // ToBase returns the value unchanged when Unit is already the base unit.
            return QuantizeForHash(ToBase(Value, Unit)).GetHashCode();
        }

        // Kept well below the 1e-12 relative comparison tolerance (tolerance-equal
        // values agree to ~12 significant digits) so they almost always round to the
        // same grid point here.
        private const int HashSignificantDigits = 10;

        private static double QuantizeForHash(double value)
        {
            if (value == 0.0)
                return 0.0;                 // normalize -0.0 and avoid Log10(0)
            if (double.IsNaN(value) || double.IsInfinity(value))
                return value;
            // Round to HashSignificantDigits significant figures (relative, to match
            // the relative comparison tolerance). value * scale is always ~1e9 in
            // magnitude regardless of value, so this neither overflows nor underflows.
            int exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)));
            double scale = Math.Pow(10, HashSignificantDigits - 1 - exponent);
            return Math.Round(value * scale) / scale;
        }

        private static bool TryParseInternal(CultureInfo cultureInfo, string text, out double value, out T unit)
        {
            value = 0;
            unit = default;

            if (text.Length < 2)
                return false;

            int lastDigit = -1;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if ((c >= '0' && c <= '9') ||
                    c == cultureInfo.NumberFormat.NumberDecimalSeparator[0] ||
                    c == cultureInfo.NumberFormat.NumberGroupSeparator[0] ||
                    c == cultureInfo.NumberFormat.NegativeSign[0] ||
                    c == '+' ||
                    c == '-' ||
                    c == ' ')
                {
                    lastDigit = i;
                }
                else
                {
                    break;
                }
            }

            if (lastDigit == text.Length - 1)
                return false;

            // Slice with spans so a successful parse allocates nothing (no Substring),
            // and use the non-throwing lookup for the routine parse-failure case.
            ReadOnlySpan<char> span = text.AsSpan();
            if (!TryParseUnit(span.Slice(lastDigit + 1), out unit))
                return false;

            return double.TryParse(span.Slice(0, lastDigit + 1), NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, out value);
        }

        /// <summary>
        /// Checks whether the measurement equals to another measurement
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Measurement<T> m)
                return Equals(m);
            return false;
        }

        /// <summary>
        /// Checks whether the measurement equals to another measurement
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Measurement<T> other)
        {
            // Same (tolerance-based) semantics as operator ==, so that == and Equals
            // agree and physically-equal measurements in different units are equal.
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Compares measurement to another measurement
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Measurement<T> other)
        {
            double v1 = In(BaseUnit);
            double v2 = other.In(BaseUnit);
            // Relative tolerance: ~50x cheaper than the previous Math.Pow/Log10 pair and,
            // unlike Math.Log10, well-defined for negative values (Log10 of a negative is
            // NaN, which disabled the tolerance entirely for negative measurements).
            if (Math.Abs(v1 - v2) <= 1e-12 * Math.Max(Math.Abs(v1), Math.Abs(v2)))
                return 0;
            return v1.CompareTo(v2);
        }


        /// <summary>
        /// Checks whether two measurements are equal
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Measurement<T> v1, Measurement<T> v2) => v1.CompareTo(v2) == 0;
        /// <summary>
        /// Checks whether two measurements are not equal
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Measurement<T> v1, Measurement<T> v2) => v1.CompareTo(v2) != 0;
        /// <summary>
        /// Checks whether the measurement is greater than another
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Measurement<T> v1, Measurement<T> v2) => v1.CompareTo(v2) > 0;
        /// <summary>
        /// Checks whether the measurement is less than another
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Measurement<T> v1, Measurement<T> v2) => v1.CompareTo(v2) < 0;
        /// <summary>
        /// Checks whether the measurement is greater than or equal to another
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Measurement<T> v1, Measurement<T> v2) => v1.CompareTo(v2) >= 0;
        /// <summary>
        /// Checks whether the measurement is less than or equal another
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Measurement<T> v1, Measurement<T> v2) => v1.CompareTo(v2) <= 0;

        /// <summary>
        /// Negates the measurement value
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> operator -(Measurement<T> v1) => new Measurement<T>(-v1.Value, v1.Unit);

        /// <summary>
        /// Unary plus value
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> operator +(Measurement<T> v1) => v1;

        /// <summary>
        /// Add one measurement to another.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> operator +(Measurement<T> v1, Measurement<T> v2) => new Measurement<T>(v1.Value + v2.In(v1.Unit), v1.Unit);
        /// <summary>
        /// Subtracts one measurement from another.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> operator -(Measurement<T> v1, Measurement<T> v2) => new Measurement<T>(v1.Value - v2.In(v1.Unit), v1.Unit);
        /// <summary>
        /// Multiples a measurement by a constant.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> operator *(Measurement<T> v1, double v2) => new Measurement<T>(v1.Value * v2, v1.Unit);

        /// <summary>
        /// Multiples a measurement by a constant.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> operator *(double v1, Measurement<T> v2) => new Measurement<T>(v2.Value * v1, v2.Unit);

        /// <summary>
        /// Divides a measurement to a specified a constant.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Measurement<T> operator /(Measurement<T> v1, double v2) => new Measurement<T>(v1.Value / v2, v1.Unit);

        /// <summary>
        /// Calculate ratio between two measurements
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double operator /(Measurement<T> v1, Measurement<T> v2) => v1.Value / v2.In(v1.Unit);

        /// <summary>
        /// Implicitly converts the value to a tuple
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Tuple<double, T>(Measurement<T> value) => new Tuple<double, T>(value.Value, value.Unit);

        /// <summary>
        /// Explicitly converts the a tuple to a value
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator Measurement<T>(Tuple<double, T>  value) => new Measurement<T>(value.Item1, value.Item2);

        /// <summary>
        /// Implicitly converts the value to an anonymous tuple
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator (double, T)(Measurement<T> value) => (value.Value, value.Unit);

        /// <summary>
        /// Implicitly converts the value to a decimal value-based measurement
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator DecimalMeasurement<T>(Measurement<T> value) => new DecimalMeasurement<T>((decimal)value.Value, value.Unit);

    }
}
