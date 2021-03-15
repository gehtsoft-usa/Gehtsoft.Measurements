using FluentAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class EnergyTest
    {
        [Theory]
        [InlineData(5.5, EnergyUnit.Joule, 4.056591821024985, EnergyUnit.FootPound)]
        [InlineData(1, EnergyUnit.BTU, 778.1280674872351, EnergyUnit.FootPound)]
        public void Conversion(double value, EnergyUnit unit, double expected, EnergyUnit targetUnit, double accurracy = 1e-10)
        {
            var v = new Measurement<EnergyUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }
    }
}
