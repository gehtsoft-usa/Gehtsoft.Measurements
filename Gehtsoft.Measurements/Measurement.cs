﻿using System;
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
            return $"{(format == "NF" ? Value.ToString() : Value.ToString(format))}{GetUnitName(Unit)}";
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
        private static readonly Func<string, T> mParseUnit =  CodeGenerator.GenerateParseUnitName<T>();
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public int GetHashCode()
        {
            double value = Value;
            if (Unit.CompareTo(BaseUnit) != 0)
                value = ToBase(Value, Unit);
            return value.GetHashCode();
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

            try
            {
                unit = ParseUnitName(text.Substring(lastDigit + 1));
            }
            catch (ArgumentException )
            {
                return false;
            }

            string n = text.Substring(0, lastDigit + 1);
            return double.TryParse(n, NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, out value);
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
            return other.In(BaseUnit) == this.In(BaseUnit);
        }

        /// <summary>
        /// Compares measurement to another measurement
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Measurement<T> other)
        {
            double v1, v2, e;
            v1 = In(BaseUnit);
            v2 = other.In(BaseUnit);
            var e1 = eps(v1);
            var e2 = eps(v2);
            e = Math.Min(e1, e2);
            if (Math.Abs(v1 - v2) < e)
                return 0;
            return v1.CompareTo(v2);
        }

        private static double eps(double value) => Math.Pow(10, Math.Round(Math.Log10(value)) - 12);


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
