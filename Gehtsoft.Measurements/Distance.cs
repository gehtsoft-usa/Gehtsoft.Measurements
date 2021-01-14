using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The class to manipulate distance/length measurements
    /// </summary>
    public readonly struct Distance 
    {
        /// <summary>
        /// The units of the distance
        /// </summary>
        public enum Unit
        {
            [Unit("ln", 1)]
            [Conversion(ConversionOperation.Divide, 10)]
            RussianLine,
            [Unit("\"", "in", 1)]
            [Conversion(ConversionOperation.Base)]
            Inch,
            [Unit("\'", "ft", 2)]
            [Conversion(ConversionOperation.Multiple, 12)]
            Foot,
            [Unit("yd", 2)]
            [Conversion(ConversionOperation.Multiple, 36)]
            Yard,
            [Unit("mi", 3)]
            [Conversion(ConversionOperation.Multiple, 63360)]
            Mile,
            [Unit("nm", 3)]
            [Conversion(ConversionOperation.Multiple, 72913.3858)]
            NauticalMile,
            [Unit("mm", 0)]
            [Conversion(ConversionOperation.Divide, 25.4)]
            Millimeter,
            [Unit("cm", 1)]
            [Conversion(ConversionOperation.Divide, 2.54)]
            Centimeter,
            [Unit("m", 1)]
            [Conversion(ConversionOperation.Divide, 25.4, ConversionOperation.Multiple, 1000)]
            Meter,
            [Unit("km", 3)]
            [Conversion(ConversionOperation.Divide, 25.4, ConversionOperation.Multiple, 1_000_000)]
            Kilometer,
        }

        /// <summary>
        /// Numerical value of the distance
        /// </summary>
        [JsonIgnore]
        public double Value { get; }

        /// <summary>
        /// The distance unit
        /// </summary>
        [JsonIgnore]
        public Unit Units { get; }

        public Distance(double value, Unit unit)
        {
            Value = value;
            Units = unit;
        }

        /// <summary>
        /// The value as a string with maximum accurracy
        /// </summary>
        [JsonPropertyName("value")]
        public string Text => ToString("N", CultureInfo.InvariantCulture);


        /// <summary>
        /// Conver to string with maximum accuracy in invariant culture
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Conver to string with maximum accuracy in the specified culture
        /// </summary>
        public string ToString(CultureInfo cultureInfo) => ToString("N", cultureInfo);

        /// <summary>
        /// Convert to string with specified format
        /// </summary>
        /// <param name="format">A numeric format or "ND" to format with the default accurracy</param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public string ToString(string format, CultureInfo cultureInfo)
        {
            if (format == "ND")
                format = $"N{GetUnitDefaultAccuracy(Units)}";
            return $"{Value}{GetUnitName(Units)}";
        }

        private static readonly Func<int, string> mGetUnitName = CodeGenerator.GenerateGetUnitName(typeof(Unit));
        private static readonly Func<string, int> mParseUnit =  CodeGenerator.GenerateParseUnitName(typeof(Unit));
        private static readonly Func<int, int> mDefaultAccuracy = CodeGenerator.GenerateGetDefaultUnitAccuracy(typeof(Unit));
        private static readonly Func<double, Unit, double> mToBase;
        private static readonly Func<double, Unit, double> mFromBase;

        /// <summary>
        /// Returns all units with their names
        /// </summary>
        /// <returns></returns>
        public static Tuple<Unit, string>[] GetUnitNames() => UnitUtils.GetUnits<Unit>(typeof(Unit));

        /// <summary>
        /// Gets the name of the unit by its code
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string GetUnitName(Unit unit) => mGetUnitName((int)unit);

        /// <summary>
        /// Gets the default accuracy of for the specified unit
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static int GetUnitDefaultAccuracy(Unit unit) => mDefaultAccuracy((int)unit);

        /// <summary>
        /// Parses the unit name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Unit ParseUnitName(string name) => (Unit)mParseUnit(name);

    }
}
