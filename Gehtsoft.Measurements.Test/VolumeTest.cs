using FluentAssertions;
using System.Globalization;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class VolumeTest
    {
        [Theory]
        [InlineData(1, VolumeUnit.CubicFeet, 1728, VolumeUnit.CubicInch)]
        [InlineData(1, VolumeUnit.CubicFeet, 0.037037, VolumeUnit.CubicYard, 1e-5)]
        [InlineData(1, VolumeUnit.CubicFeet, 28.31684, VolumeUnit.Liter, 1e-5)]
        [InlineData(1, VolumeUnit.CubicFeet, 0.028316, VolumeUnit.CubicMeter, 1e-5)]
        [InlineData(1, VolumeUnit.CubicFeet, 7.480519, VolumeUnit.Gallon, 1e-5)]
        public void Conversion(double value, VolumeUnit unit, double expected, VolumeUnit targetUnit, double accurracy = 1e-10)
        {
            Measurement<VolumeUnit> v = new Measurement<VolumeUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }

        [Theory]
        [InlineData("15.4m³", 15.4, VolumeUnit.CubicMeter)]
        [InlineData("15.4m3", 15.4, VolumeUnit.CubicMeter)]
        public void Parse(string text, double value, VolumeUnit unit)
        {
            Measurement<VolumeUnit>.TryParse(CultureInfo.InvariantCulture, text, out Measurement<VolumeUnit> v).Should().BeTrue();
            v.Value.Should().BeApproximately(value, 1e-10);
            v.Unit.Should().Be(unit);
        }

    }
}
