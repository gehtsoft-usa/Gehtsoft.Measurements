using FluentAssertions;
using System.Globalization;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class TemperatureTest
    {
        [Theory]
        [InlineData(50, TemperatureUnit.Fahrenheit, 10, TemperatureUnit.Celsius, 1e-5)]
        [InlineData(-50, TemperatureUnit.Fahrenheit, 227.5944444, TemperatureUnit.Kelvin, 1e-5)]
        [InlineData(38, TemperatureUnit.Celsius, 560.07, TemperatureUnit.Rankin, 1e-5)]
        [InlineData(117, TemperatureUnit.Fahrenheit, 576.67, TemperatureUnit.Rankin, 1e-5)]
        public void Conversion(double value, TemperatureUnit unit, double expected, TemperatureUnit targetUnit, double accurracy = 1e-10)
        {
            Measurement<TemperatureUnit> v = new Measurement<TemperatureUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }

        [Theory]
        [InlineData("36.6C", 36.6, TemperatureUnit.Celsius)]
        [InlineData("36.6°C", 36.6, TemperatureUnit.Celsius)]
        [InlineData("-36.6°F", -36.6, TemperatureUnit.Fahrenheit)]
        [InlineData("366°K", 366, TemperatureUnit.Kelvin)]
        public void Parse(string text, double value, TemperatureUnit unit)
        {
            Measurement<TemperatureUnit>.TryParse(CultureInfo.InvariantCulture, text, out Measurement<TemperatureUnit> v).Should().BeTrue();
            v.Value.Should().BeApproximately(value, 1e-10);
            v.Unit.Should().Be(unit);
        }

    }
}
