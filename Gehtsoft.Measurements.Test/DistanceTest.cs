using FluentAssertions;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Dynamic;
using System.IO;
using Binaron.Serializer;
using System.Xml.Serialization;

namespace Gehtsoft.Measurements.Test
{
    public class DistanceTest
    {
        [Theory]
        [InlineData(DistanceUnit.Inch, "\"", "in")]
        [InlineData(DistanceUnit.Foot, "\'", "ft")]
        [InlineData(DistanceUnit.Yard, "yd", null)]
        [InlineData(DistanceUnit.RussianLine, "rln", null)]
        public void UnitNames(DistanceUnit unit, string name, string altName)
        {
            Measurement<DistanceUnit>.GetUnitName(unit).Should().Be(name);
            Measurement<DistanceUnit>.ParseUnitName(name).Should().Be(unit);
            if (!string.IsNullOrEmpty(altName))
                Measurement<DistanceUnit>.ParseUnitName(altName).Should().Be(unit);

            Action action = () => Measurement<DistanceUnit>.GetUnitName((DistanceUnit)1000);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UnitNamesList()
        {
            var names = Measurement<DistanceUnit>.GetUnitNames();
            names.Should().Contain(new Tuple<DistanceUnit, string>(DistanceUnit.Inch, "\""));
            names.Should().Contain(new Tuple<DistanceUnit, string>(DistanceUnit.Centimeter, "cm"));
        }

        [Theory]
        [InlineData("iv", "1,234.24567ft", 1234.24567, DistanceUnit.Foot)]
        [InlineData("ru", "-1 234,24567'", -1234.24567, DistanceUnit.Foot)]

        public void Parser(string culture, string text, double value, DistanceUnit unit)
        {
            CultureInfo ci = culture == "iv" ? CultureInfo.InvariantCulture : CultureInfo.GetCultures(CultureTypes.AllCultures).First(c => c.Name == culture);
            ci.Should().NotBeNull();
            Measurement<DistanceUnit>.TryParse(ci, text, out Measurement<DistanceUnit> v).Should().BeTrue();
            v.Value.Should().Be(value);
            v.Unit.Should().Be(unit);
        }


        [Theory]
        [InlineData(1, DistanceUnit.Inch, 1)]
        [InlineData(1, DistanceUnit.RussianLine, 0.1)]
        [InlineData(1, DistanceUnit.Centimeter, 0.3937007874015748031496062992126)]
        [InlineData(1, DistanceUnit.Meter, 39.37007874015748031496062992126)]
        public void StaticConversion(double value, DistanceUnit unit, double expected)
        {
            Measurement<DistanceUnit>.ToBase(value, unit).Should().BeApproximately(expected, 1e-10);
            Measurement<DistanceUnit>.FromBase(expected, unit).Should().BeApproximately(value, 1e-10);
        }

        [Theory]
        [InlineData(1, DistanceUnit.Inch, 2.54, DistanceUnit.Centimeter)]
        [InlineData(10, DistanceUnit.RussianLine, 1, DistanceUnit.Inch)]
        [InlineData(10, DistanceUnit.RussianLine, 2.54, DistanceUnit.Centimeter)]
        public void In(double value, DistanceUnit unit, double expected, DistanceUnit targetUnit)
        {
            Measurement<DistanceUnit> v = new Measurement<DistanceUnit>(value, unit);
            v.In(targetUnit).Should().Be(expected);
        }

        [Theory]
        [InlineData(1, DistanceUnit.Inch, 2.54, DistanceUnit.Centimeter, 0)]
        [InlineData(2, DistanceUnit.Inch, 2.54, DistanceUnit.Centimeter, 1)]
        [InlineData(0.5, DistanceUnit.Inch, 2.54, DistanceUnit.Centimeter, -1)]
        public void Compare(double value1, DistanceUnit unit1, double value2, DistanceUnit unit2, int expectedResult)
        {
            Measurement<DistanceUnit> v1 = new Measurement<DistanceUnit>(value1, unit1);
            Measurement<DistanceUnit> v2 = new Measurement<DistanceUnit>(value2, unit2);
            if (expectedResult == 0)
                v1.CompareTo(v2).Should().Be(0);
            else if (expectedResult > 0)
                v1.CompareTo(v2).Should().BeGreaterThan(0);
            else if (expectedResult < 0)
                v1.CompareTo(v2).Should().BeLessThan(0);
        }

        [Fact]
        public void SerializationJson()
        {
            Measurement<DistanceUnit> v = new Measurement<DistanceUnit>(1245, DistanceUnit.Meter);
            string s = JsonSerializer.Serialize(v);
            s.Should().Be("{\"value\":\"1245m\"}");
            var v1 = JsonSerializer.Deserialize<Measurement<DistanceUnit>>(s);
            v1.Value.Should().Be(1245);
            v1.Unit.Should().Be(DistanceUnit.Meter);

        }

        [Fact]
        public void SerializationXml()
        {
            Measurement<DistanceUnit> v = new Measurement<DistanceUnit>(1245, DistanceUnit.Meter);
            XmlSerializer serializer = new XmlSerializer(typeof(Measurement<DistanceUnit>));
            string raw;
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, v);
                raw = writer.ToString();
            }
            
            using (StringReader r = new StringReader(raw))
            {
                var v1 = (Measurement<DistanceUnit>)serializer.Deserialize(r);
                v1.Value.Should().Be(1245);
                v1.Unit.Should().Be(DistanceUnit.Meter);
            }
        }

        [Fact]
        public void SerializationBinaron()
        {
            Measurement<DistanceUnit> v = new Measurement<DistanceUnit>(1245, DistanceUnit.Meter);
            byte[] arr;

            using (var ms = new MemoryStream())
            {
                BinaronConvert.Serialize<Measurement<DistanceUnit>>(v, ms);
                arr = ms.ToArray();
            }


            using (var ms = new MemoryStream(arr))
            {
                var v1 = BinaronConvert.Deserialize<Measurement<DistanceUnit>>(ms);
                v1.Value.Should().Be(1245);
                v1.Unit.Should().Be(DistanceUnit.Meter);
            }
        }

    }
}
