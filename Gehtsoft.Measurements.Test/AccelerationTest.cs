using FluentAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class AccelerationTest
    {
        [Theory]
        [InlineData(3.28084, AccelerationUnit.FeetPerSecondSquare, 0.101972, AccelerationUnit.EarthGravity, 1e-5)]
        [InlineData(1, AccelerationUnit.EarthGravity, 9.80665, AccelerationUnit.MeterPerSecondSquare, 1e-5)]
        [InlineData(1, AccelerationUnit.EarthGravity, 32.174048, AccelerationUnit.FeetPerSecondSquare, 1e-5)]
        public void Conversion(double value, AccelerationUnit unit, double expected, AccelerationUnit targetUnit, double accurracy = 1e-10)
        {
            var v = new Measurement<AccelerationUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }
    }
}
