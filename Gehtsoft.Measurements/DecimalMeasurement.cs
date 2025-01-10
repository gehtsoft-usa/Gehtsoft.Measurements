using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

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
    public readonly struct DecimalMeasurement<T> : IEquatable<DecimalMeasurement<T>>, IComparable<DecimalMeasurement<T>>, IFormattable
        where T : Enum
    {
        /// <summary>
        /// Numerical value
        /// </summary>
        [JsonIgnore]
        public readonly decimal Value;

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
        public DecimalMeasurement(decimal value, T unit)
        {
            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Constructor that accepts a tuple.
        /// </summary>
        /// <param name="value"></param>
        public DecimalMeasurement(Tuple<decimal, T> value)
        {
            Value = value.Item1;
            Unit = value.Item2;
        }

        /// <summary>
        /// Constructor that accepts a anonymous tuple.
        /// </summary>
        /// <param name="value"></param>
        public DecimalMeasurement((decimal, T) value)
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
        public DecimalMeasurement(string text)
        {
            if (!DecimalMeasurement<T>.TryParseInternal(CultureInfo.InvariantCulture, text, out decimal value, out T unit))
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
            return $"{(format == "NF" ? Value.ToString() : Value.ToString(format))}{GetUnitName(Unit)}";
        }

        /// <summary>
        /// Returns the value in the specified units
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal In(T unit) => Convert(Value, Unit, unit);

        /// <summary>
        /// Converts the value into another unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DecimalMeasurement<T> To(T unit) => new DecimalMeasurement<T>(In(unit), unit);

        /// <summary>
        /// Convert value from one unit to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Convert(decimal value, T from, T to)
        {
            if (from.CompareTo(to) == 0)
                return value;
            if (from.CompareTo(BaseUnit) != 0)
                value = ToBase(value, from);
            if (to.CompareTo(BaseUnit) != 0)
                value = FromBase(value, to);
            return value;
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
        private static readonly Func<string, T> mParseUnit = CodeGenerator.GenerateParseUnitName<T>();
        private static readonly Func<T, int> mDefaultAccuracy = CodeGenerator.GenerateGetDefaultUnitAccuracy<T>();
        private static readonly Func<decimal, T, decimal> mToBaseDecimal = CodeGenerator.GenerateConversionDecimal<T>(true);
        private static readonly Func<decimal, T, decimal> mFromBaseDecimal = CodeGenerator.GenerateConversionDecimal<T>(false);

        /// <summary>
        /// Converts the value from the specified units to a base unit.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ToBase(decimal value, T unit) => mToBaseDecimal(value, unit);

        /// <summary>
        /// Converts the value to the specified unit  from a base unit.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal FromBase(decimal value, T unit) => mFromBaseDecimal(value, unit);

        /// <summary>
        /// Returns all units with their names
        /// </summary>
        /// <returns></returns>
        public static Tuple<T, string>[] GetUnitNames() => UnitUtils.GetUnits<T>();

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
        public static T ParseUnitName(string name) => mParseUnit(name);

        /// <summary>
        /// Try to parse the value using the current culture
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(string text, out DecimalMeasurement<T> value) => TryParse(CultureInfo.CurrentCulture, text, out value);

        /// <summary>
        /// Try to parse the value using the specified culture
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParse(CultureInfo cultureInfo, string text, out DecimalMeasurement<T> value)
        {
            bool rc = TryParseInternal(cultureInfo, text, out decimal _value, out T unit);
            if (rc)
                value = new DecimalMeasurement<T>(_value, unit);
            else
                value = new DecimalMeasurement<T>(0m, default);
            return rc;
        }

        /// <summary>
        /// Returns hash code of the value
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public int GetHashCode()
        {
            decimal value = Value;
            if (Unit.CompareTo(BaseUnit) != 0)
                value = ToBase(Value, Unit);
            return value.GetHashCode();
        }

        private static bool TryParseInternal(CultureInfo cultureInfo, string text, out decimal value, out T unit)
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

            try
            {
                unit = ParseUnitName(text.Substring(lastDigit + 1));
            }
            catch (ArgumentException)
            {
                return false;
            }

            string n = text.Substring(0, lastDigit + 1);
            return decimal.TryParse(n, NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, out value);
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
            if (obj is DecimalMeasurement<T> m)
                return Equals(m);
            return false;
        }

        /// <summary>
        /// Checks whether the measurement equals to another measurement
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DecimalMeasurement<T> other)
        {
            return other.In(BaseUnit) == this.In(BaseUnit);
        }

        /// <summary>
        /// Compares measurement to another measurement
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(DecimalMeasurement<T> other) 
        {
            decimal v1, v2;
            v1 = In(BaseUnit);
            v2 = other.In(BaseUnit);
            return v1.CompareTo(v2);
        }

        /// <summary>
        /// Checks whether two measurements are equal
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2) => v1.CompareTo(v2) == 0;
        /// <summary>
        /// Checks whether two measurements are not equal
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2) => v1.CompareTo(v2) != 0;
        /// <summary>
        /// Checks whether the measurement is greater than another
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2) => v1.CompareTo(v2) > 0;
        /// <summary>
        /// Checks whether the measurement is less than another
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2) => v1.CompareTo(v2) < 0;
        /// <summary>
        /// Checks whether the measurement is greater than or equal to another
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2) => v1.CompareTo(v2) >= 0;
        /// <summary>
        /// Checks whether the measurement is less than or equal another
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2) => v1.CompareTo(v2) <= 0;

        /// <summary>
        /// Negates the measurement value
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> operator -(DecimalMeasurement<T> v1) => new DecimalMeasurement<T>(-v1.Value, v1.Unit);

        /// <summary>
        /// Unary plus value
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> operator +(DecimalMeasurement<T> v1) => v1;

        /// <summary>
        /// Add one measurement to another.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> operator +(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2) => new DecimalMeasurement<T>(v1.Value + v2.In(v1.Unit), v1.Unit);
        /// <summary>
        /// Subtracts one measurement from another.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> operator -(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2) => new DecimalMeasurement<T>(v1.Value - v2.In(v1.Unit), v1.Unit);
        /// <summary>
        /// Multiples a measurement by a constant.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> operator *(DecimalMeasurement<T> v1, decimal v2) => new DecimalMeasurement<T>(v1.Value * v2, v1.Unit);

        /// <summary>
        /// Multiples a measurement by a constant.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> operator *(decimal v1, DecimalMeasurement<T> v2) => new DecimalMeasurement<T>(v2.Value * v1, v2.Unit);

        /// <summary>
        /// Divides a measurement to a specified a constant.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DecimalMeasurement<T> operator /(DecimalMeasurement<T> v1, decimal v2) => new DecimalMeasurement<T>(v1.Value / v2, v1.Unit);

        /// <summary>
        /// Calculate ratio between two measurements
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal operator /(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2) => v1.Value / v2.In(v1.Unit);

        /// <summary>
        /// Implicitly converts the value to a tuple
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Tuple<decimal, T>(DecimalMeasurement<T> value) => new Tuple<decimal, T>(value.Value, value.Unit);

        /// <summary>
        /// Explicitly converts the a tuple to a value
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator DecimalMeasurement<T>(Tuple<decimal, T> value) => new DecimalMeasurement<T>(value.Item1, value.Item2);

        /// <summary>
        /// Implicitly converts the value to an anonymous tuple
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator (decimal, T)(DecimalMeasurement<T> value) => (value.Value, value.Unit);

        /// <summary>
        /// Implicitly converts the value to a double value-based measurement
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Measurement<T>(DecimalMeasurement<T> value) => new Measurement<T>((double)value.Value, value.Unit);
    }
}
