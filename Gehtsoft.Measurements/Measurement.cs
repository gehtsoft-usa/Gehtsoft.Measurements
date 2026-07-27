using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
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
    public readonly struct Measurement<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T> : IEquatable<Measurement<T>>, IComparable<Measurement<T>>,
                                            IFormattable, ISpanFormattable, IUtf8SpanFormattable,
                                            IParsable<Measurement<T>>, ISpanParsable<Measurement<T>>,
                                            IAdditionOperators<Measurement<T>, Measurement<T>, Measurement<T>>,
                                            ISubtractionOperators<Measurement<T>, Measurement<T>, Measurement<T>>,
                                            IUnaryNegationOperators<Measurement<T>, Measurement<T>>,
                                            IUnaryPlusOperators<Measurement<T>, Measurement<T>>,
                                            IComparisonOperators<Measurement<T>, Measurement<T>, bool>,
                                            IMultiplyOperators<Measurement<T>, double, Measurement<T>>,
                                            IDivisionOperators<Measurement<T>, double, Measurement<T>>,
                                            IDivisionOperators<Measurement<T>, Measurement<T>, double>
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
        /// <para>Constructor that accepts a text representation of a value</para>
        /// <para>The text is always parsed in the invariant culture, so that the value round-trips through the `Text` property.</para>
        /// </summary>
        /// <param name="text"></param>
        [JsonConstructor]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Measurement(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (!Measurement<T>.TryParseInternal(text.AsSpan(), NumberFormatInfo.InvariantInfo, out double value, out T unit))
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
        /// Convert to string with the specified format in invariant culture
        /// </summary>
        /// <param name="format">A numeric format or `"ND"` to format with the default accuracy and `"NF"` to display as all digits after decimal point</param>
        /// <returns></returns>
        public string ToString(string format) => ToString(format, CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert to string with specified format
        /// </summary>
        /// <param name="format">A numeric format or `"ND"` to format with the default accuracy and `"NF"` to display as all digits after decimal point</param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            // The buffer holds any realistic value, so the formatting itself is done without
            // intermediate strings and only the result is allocated. The path below it stays
            // for the formats which can overflow the buffer, such as `"N100"` of a huge value.
            Span<char> buffer = stackalloc char[StackFormatBufferLength];
            if (TryFormat(buffer, out int charsWritten, format.AsSpan(), formatProvider))
                return new string(buffer.Slice(0, charsWritten));

            if (format == "ND")
                format = UnitUtils.AccuracyFormat(GetUnitDefaultAccuracy(Unit));
            return $"{(format == "NF" ? Value.ToString(formatProvider) : Value.ToString(format, formatProvider))}{GetUnitName(Unit)}";
        }

        private const int StackFormatBufferLength = 512;

        /// <summary>
        /// Formats the value into a character span.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="charsWritten"></param>
        /// <param name="format">A numeric format or `"ND"` to format with the default accuracy and `"NF"` to display as all digits after decimal point</param>
        /// <param name="formatProvider"></param>
        /// <returns>`false` if the destination is too small to hold the whole value.</returns>
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider formatProvider)
        {
            charsWritten = 0;

            if (!Value.TryFormat(destination, out int valueLength, ValueFormat(format), formatProvider))
                return false;

            string unitName = GetUnitName(Unit);
            if (!unitName.AsSpan().TryCopyTo(destination.Slice(valueLength)))
                return false;

            charsWritten = valueLength + unitName.Length;
            return true;
        }

        /// <summary>
        /// Formats the value into a span of UTF-8 bytes.
        /// </summary>
        /// <param name="utf8Destination"></param>
        /// <param name="bytesWritten"></param>
        /// <param name="format">A numeric format or `"ND"` to format with the default accuracy and `"NF"` to display as all digits after decimal point</param>
        /// <param name="formatProvider"></param>
        /// <returns>`false` if the destination is too small to hold the whole value.</returns>
        public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider formatProvider)
        {
            bytesWritten = 0;

            if (!Value.TryFormat(utf8Destination, out int valueLength, ValueFormat(format), formatProvider))
                return false;

            // Unit names are not all ASCII (degrees, superscripts, the middle dot), so the name
            // is transcoded rather than copied. It is written straight into the destination.
            if (!Encoding.UTF8.TryGetBytes(GetUnitName(Unit).AsSpan(), utf8Destination.Slice(valueLength), out int nameLength))
                return false;

            bytesWritten = valueLength + nameLength;
            return true;
        }

        // Translates the two measurement-specific formats into the numeric format to apply to the
        // value itself. An empty format is the general format, which is what `"NF"` means here.
        private ReadOnlySpan<char> ValueFormat(ReadOnlySpan<char> format)
        {
            if (format.SequenceEqual("ND".AsSpan()))
                return UnitUtils.AccuracyFormat(GetUnitDefaultAccuracy(Unit)).AsSpan();
            if (format.SequenceEqual("NF".AsSpan()))
                return default;
            return format;
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
        /// Try to parse the value from a character span using the current culture
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(ReadOnlySpan<char> text, out Measurement<T> value) => TryParse(CultureInfo.CurrentCulture, text, out value);

        /// <summary>
        /// Try to parse the value using the specified culture
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(CultureInfo cultureInfo, string text, out Measurement<T> value) => TryParse(cultureInfo, text.AsSpan(), out value);

        /// <summary>
        /// Try to parse the value from a character span using the specified culture
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(CultureInfo cultureInfo, ReadOnlySpan<char> text, out Measurement<T> value)
            => TryParseCore(text, NumberFormatInfo.GetInstance(cultureInfo), out value);

        /// <summary>
        /// Try to parse the value using the specified format provider
        /// </summary>
        /// <param name="text"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(string text, IFormatProvider provider, out Measurement<T> value)
            => TryParseCore(text.AsSpan(), NumberFormatInfo.GetInstance(provider), out value);

        /// <summary>
        /// Try to parse the value from a character span using the specified format provider
        /// </summary>
        /// <param name="text"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(ReadOnlySpan<char> text, IFormatProvider provider, out Measurement<T> value)
            => TryParseCore(text, NumberFormatInfo.GetInstance(provider), out value);

        private static bool TryParseCore(ReadOnlySpan<char> text, NumberFormatInfo numberFormat, out Measurement<T> value)
        {
            bool rc = TryParseInternal(text, numberFormat, out double _value, out T unit);
            if (rc)
                value = new Measurement<T>(_value, unit);
            else
                value = new Measurement<T>(0, default);
            return rc;
        }

        /// <summary>
        /// Parses the value using the current culture
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Measurement<T> Parse(string text) => Parse(text.AsSpan(), CultureInfo.CurrentCulture);

        /// <summary>
        /// Parses the value from a character span using the current culture
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Measurement<T> Parse(ReadOnlySpan<char> text) => Parse(text, CultureInfo.CurrentCulture);

        /// <summary>
        /// Parses the value using the specified format provider
        /// </summary>
        /// <param name="text"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Measurement<T> Parse(string text, IFormatProvider provider) => Parse(text.AsSpan(), provider);

        /// <summary>
        /// Parses the value from a character span using the specified format provider
        /// </summary>
        /// <param name="text"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Measurement<T> Parse(ReadOnlySpan<char> text, IFormatProvider provider)
        {
            if (TryParseCore(text, NumberFormatInfo.GetInstance(provider), out Measurement<T> value))
                return value;
            throw new FormatException("The text is not a valid measurement value");
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

        // The whole parse runs on spans and on the pre-built unit list, so it allocates nothing
        // on either the success or the failure path. A null string reaches this as an empty
        // span, which is rejected by the length check rather than throwing.
        private static bool TryParseInternal(ReadOnlySpan<char> text, NumberFormatInfo numberFormat, out double value, out T unit)
        {
            value = 0;
            unit = default;

            if (text.Length < 2)
                return false;

            // The unit is looked for at the end of the text rather than the number at its
            // start, so that a number written in the exponent notation ("1e3m") is not cut in
            // the middle. The longest name which ends the text wins; if what is left in front
            // of it is not a number, the search goes on with the next shorter name. That
            // retry is what keeps names which hold digits or separators ("in/100yd",
            // "l/100km") and names which end with another name ("mrad" over "rad") unambiguous.
            char lastChar = text[text.Length - 1];
            int maxLength = text.Length;
            while (true)
            {
                int nameLength = -1;
                T candidate = default;

                var list = mParseList;
                for (int i = 0; i < list.Length; i++)
                {
                    string name = list[i].Name;

                    // at least one character must be left for the value itself
                    if (name.Length >= maxLength || name.Length <= nameLength)
                        continue;

                    // comparing the last character first keeps a text which ends with no known
                    // unit at all - the routine parse failure - down to one character compare
                    // per unit instead of a span comparison per unit
                    if (name[name.Length - 1] != lastChar)
                        continue;

                    if (text.EndsWith(name.AsSpan()))
                    {
                        nameLength = name.Length;
                        candidate = list[i].Unit;
                    }
                }

                if (nameLength < 0)
                    return false;

                if (double.TryParse(text.Slice(0, text.Length - nameLength), NumberStyles.Float | NumberStyles.AllowThousands, numberFormat, out value))
                {
                    unit = candidate;
                    return true;
                }

                maxLength = nameLength;
            }
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
