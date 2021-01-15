using FluentAssertions;
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
            Measurement<VelocityUnit> v = new Measurement<VelocityUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }
    }
}
