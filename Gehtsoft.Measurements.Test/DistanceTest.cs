using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class DistanceTest
    {
        [Theory]
        [InlineData(Distance.Unit.Inch, "\"", "in")]
        [InlineData(Distance.Unit.Foot, "\'", "ft")]
        [InlineData(Distance.Unit.Yard, "yd", null)]
        [InlineData(Distance.Unit.RussianLine, "ln", null)]
        public void TestUnitNames(Distance.Unit unit, string name, string altName)
        {
            Distance.GetUnitName(unit).Should().Be(name);
            Distance.ParseUnitName(name).Should().Be(unit);
            if (!string.IsNullOrEmpty(altName))
                Distance.ParseUnitName(altName).Should().Be(unit);

            Action action = () => Distance.GetUnitName((Distance.Unit)1000);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void TestUnitNamesList()
        {
            var names = Distance.GetUnitNames();
            names.Should().Contain(new Tuple<Distance.Unit, string>(Distance.Unit.Inch, "\""));
            names.Should().Contain(new Tuple<Distance.Unit, string>(Distance.Unit.Centimeter, "cm"));
        }

    }
}
