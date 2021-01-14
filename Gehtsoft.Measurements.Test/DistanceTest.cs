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

    }
}
