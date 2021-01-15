﻿using FluentAssertions;
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
        [InlineData(DistanceUnit.RussianLine, "ln", null)]
        public void TestUnitNames(DistanceUnit unit, string name, string altName)
        {
            Measurement<DistanceUnit>.GetUnitName(unit).Should().Be(name);
            Measurement<DistanceUnit>.ParseUnitName(name).Should().Be(unit);
            if (!string.IsNullOrEmpty(altName))
                Measurement<DistanceUnit>.ParseUnitName(altName).Should().Be(unit);

            Action action = () => Measurement<DistanceUnit>.GetUnitName((DistanceUnit)1000);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void TestUnitNamesList()
        {
            var names = Measurement<DistanceUnit>.GetUnitNames();
            names.Should().Contain(new Tuple<DistanceUnit, string>(DistanceUnit.Inch, "\""));
            names.Should().Contain(new Tuple<DistanceUnit, string>(DistanceUnit.Centimeter, "cm"));
        }

        [Theory]
        [InlineData("iv", "1,234.24567ft", 1234.24567, DistanceUnit.Foot)]
        [InlineData("ru", "-1 234,24567'", -1234.24567, DistanceUnit.Foot)]

        public void TestParser(string culture, string text, double value, DistanceUnit unit)
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
        public void TestToBaseStatic(double value, DistanceUnit unit, double expected)
        {
            Measurement<DistanceUnit>.ToBase(value, unit).Should().BeApproximately(expected, 1e-10);
        }

        [Fact]
        public void JsonTest()
        {
            Measurement<DistanceUnit> v = new Measurement<DistanceUnit>(1245, DistanceUnit.Meter);
            string s = JsonSerializer.Serialize(v);
            s.Should().Be("{\"value\":\"1245m\"}");
            var v1 = JsonSerializer.Deserialize<Measurement<DistanceUnit>>(s);
            v1.Value.Should().Be(1245);
            v1.Unit.Should().Be(DistanceUnit.Meter);

        }

        [Fact]
        public void XmlTest()
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
        public void BinaronTest()
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
