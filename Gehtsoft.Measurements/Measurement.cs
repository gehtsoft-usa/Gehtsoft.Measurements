using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The class to manipulate measurements expressed in the specified units
    /// </summary>
    public readonly struct Measurement<T>
        where T : Enum
    {
        /// <summary>
        /// Numerical value
        /// </summary>
        [JsonIgnore]
        public double Value { get; }

        /// <summary>
        /// The unit
        /// </summary>
        [JsonIgnore]
        public T Unit { get; }

        public Measurement(double value, T unit)
        {
            Value = value;
            Unit = unit;
        }

        [JsonConstructor]
        public Measurement(string text)
        {
            if (!Measurement<T>.TryParse(CultureInfo.InvariantCulture, text, out Measurement<T> x))
                throw new ArgumentException("Invalid value", nameof(text));
            Value = x.Value;
            Unit = x.Unit;
        }

        /// <summary>
        /// The value as a string with maximum accuracy
        /// </summary>
        [JsonPropertyName("value")]
        public string Text => ToString("N", CultureInfo.InvariantCulture);


        /// <summary>
        /// Convert to string with maximum accuracy in invariant culture
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert to string with maximum accuracy in the specified culture
        /// </summary>
        public string ToString(CultureInfo cultureInfo) => ToString("N", cultureInfo);

        /// <summary>
        /// Convert to string with specified format
        /// </summary>
        /// <param name="format">A numeric format or "ND" to format with the default accuracy</param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public string ToString(string format, CultureInfo cultureInfo)
        {
            if (format == "ND")
                format = $"N{GetUnitDefaultAccuracy(Unit)}";
            return $"{Value}{GetUnitName(Unit)}";
        }

        private static readonly Func<int, string> mGetUnitName = CodeGenerator.GenerateGetUnitName(typeof(DistanceUnit));
        private static readonly Func<string, int> mParseUnit =  CodeGenerator.GenerateParseUnitName(typeof(DistanceUnit));
        private static readonly Func<int, int> mDefaultAccuracy = CodeGenerator.GenerateGetDefaultUnitAccuracy(typeof(DistanceUnit));
        private static readonly Func<double, DistanceUnit, double> mToBase;
        private static readonly Func<double, DistanceUnit, double> mFromBase;

        /// <summary>
        /// Returns all units with their names
        /// </summary>
        /// <returns></returns>
        public static Tuple<T, string>[] GetUnitNames() => UnitUtils.GetUnits<T>(typeof(T));

        /// <summary>
        /// Gets the name of the unit by its code
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string GetUnitName(T unit) => mGetUnitName((int)Convert.ChangeType(unit, TypeCode.Int32));

        /// <summary>
        /// Gets the default accuracy of for the specified unit
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static int GetUnitDefaultAccuracy(T unit) => mDefaultAccuracy((int)Convert.ChangeType(unit, TypeCode.Int32));

        /// <summary>
        /// Parses the unit name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T ParseUnitName(string name) => (T)Enum.ToObject(typeof(T), mParseUnit(name));

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
            if (text.Length < 2)
            {
                value = new Measurement<T>(0, default(T));
                return false;
            }

            int lastDigit = -1; ;
            for (int i = text.Length - 1; i >= 0 && lastDigit == -1; i--)
                if (text[i] >= '0' && text[i] <= '9' || text[i] == '.' || text[i] == ',')
                    lastDigit = i;

            if (lastDigit == text.Length - 1)
            {
                value = new Measurement<T>(0, default(T));
                return false;
            }


            T unit;
            try
            {
                unit = ParseUnitName(text.Substring(lastDigit + 1));
            }
            catch (ArgumentException e)
            {
                value = new Measurement<T>(0, default(T));
                return false;
            }

            double v = 0;

            string n = text.Substring(0, lastDigit + 1);
            if (!double.TryParse(n, NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, out v))
            {
                value = new Measurement<T>(0, default(T));
                return false;
            }    

            value = new Measurement<T>(v, unit);
            return true;
        }
    }
}
