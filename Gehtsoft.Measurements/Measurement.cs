using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Gehtsoft.Measurements
{
    /// <summary>
    /// The class to manipulate measurements expressed in the specified units.
    /// 
    /// The class supports serialization using System.Text.Json serializer and XmlSerializer as well as 
    /// many 3rd party serializers such as BinaronSerializer.
    /// </summary>
    public struct Measurement<T> : IXmlSerializable
        where T : Enum
    {
        /// <summary>
        /// Numerical value
        /// </summary>
        [JsonIgnore]
        public double Value { get; private set; }

        /// <summary>
        /// The unit
        /// </summary>
        [JsonIgnore]
        public T Unit { get; private set; }

        public Measurement(double value, T unit)
        {
            Value = value;
            Unit = unit;
        }

        [JsonConstructor]
        public Measurement(string text)
        {
            Value = 0;
            Unit = default(T);
            if (!Measurement<T>.TryParseInternal(CultureInfo.InvariantCulture, text, ref this)) 
                throw new ArgumentException("Invalid value", nameof(text));
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

        private static readonly Func<T, string> mGetUnitName = CodeGenerator.GenerateGetUnitName<T>();
        private static readonly Func<string, T> mParseUnit =  CodeGenerator.GenerateParseUnitName<T>();
        private static readonly Func<T, int> mDefaultAccuracy = CodeGenerator.GenerateGetDefaultUnitAccuracy<T>();
        private static readonly Func<double, T, double> mToBase = CodeGenerator.GenerateToBaseConversion<T>();
        private static readonly Func<double, int, double> mFromBase;

        public static double ToBase(double value, T unit) => mToBase(value, unit);

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
        public static string GetUnitName(T unit) => mGetUnitName(unit);

        /// <summary>
        /// Gets the default accuracy of for the specified unit
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
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
            value = new Measurement<T>(0, default(T));
            return TryParseInternal(cultureInfo, text, ref value);
        }

        private static bool TryParseInternal(CultureInfo cultureInfo, string text, ref Measurement<T> value)
        {
            if (text.Length < 2)
                return false;

            int lastDigit = -1; ;
            for (int i = text.Length - 1; i >= 0 && lastDigit == -1; i--)
                if (text[i] >= '0' && text[i] <= '9' || text[i] == '.' || text[i] == ',')
                    lastDigit = i;

            if (lastDigit == text.Length - 1)
                return false;


            T unit;
            try
            {
                unit = ParseUnitName(text.Substring(lastDigit + 1));
            }
            catch (ArgumentException e)
            {
                return false;
            }

            double v = 0;

            string n = text.Substring(0, lastDigit + 1);
            if (!double.TryParse(n, NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, out v))
            {
                value = new Measurement<T>(0, default(T));
                return false;
            }

            value.Value = v;
            value.Unit = unit;
            return true;
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToAttribute("value");
            reader.ReadAttributeValue();
            string s = reader.Value;
            TryParseInternal(CultureInfo.InvariantCulture, s, ref this);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("value", Text);
        }
    }
}
