using AwesomeAssertions;
using System.Globalization;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class VelocityTest
    {
        [Theory]
        [InlineData(5, VelocityUnit.MilesPerHour, 7.33333333, VelocityUnit.FeetPerSecond)]
        [InlineData(12, VelocityUnit.MetersPerSecond, 43.2, VelocityUnit.KilometersPerHour)]
        [InlineData(12.5, VelocityUnit.Knot, 21.09762, VelocityUnit.FeetPerSecond)]
        [InlineData(2700, VelocityUnit.FeetPerSecond, 822.96, VelocityUnit.MetersPerSecond)]
        public void Conversion(double value, VelocityUnit unit, double expected, VelocityUnit targetUnit, double accurracy = 1e-5)
        {
            var v = new Measurement<VelocityUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }

        [Theory]
        [InlineData("12.5m/s", 12.5, VelocityUnit.MetersPerSecond)]
        [InlineData("12.5mps", 12.5, VelocityUnit.MetersPerSecond)]
        [InlineData("12.5ft/s", 12.5, VelocityUnit.FeetPerSecond)]
        [InlineData("12.5fps", 12.5, VelocityUnit.FeetPerSecond)]
        [InlineData("12.5km/h", 12.5, VelocityUnit.KilometersPerHour)]
        [InlineData("12.5kmph", 12.5, VelocityUnit.KilometersPerHour)]
        [InlineData("12.5mi/h", 12.5, VelocityUnit.MilesPerHour)]
        [InlineData("12.5mph", 12.5, VelocityUnit.MilesPerHour)]
        public void Parse(string text, double value, VelocityUnit unit)
        {
            Measurement<VelocityUnit>.TryParse(CultureInfo.InvariantCulture, text, out Measurement<VelocityUnit> v).Should().BeTrue();
            v.Value.Should().BeApproximately(value, 1e-10);
            v.Unit.Should().Be(unit);
        }
    }
}
