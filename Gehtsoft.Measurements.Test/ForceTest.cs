using FluentAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    public class ForceTest
    {
        [Theory]
        [InlineData(1, ForceUnit.Newton, 100000, ForceUnit.Dyne, 1e-5)]
        [InlineData(1, ForceUnit.Newton, 0.10197, ForceUnit.KilogramForce, 1e-5)]
        [InlineData(1, ForceUnit.Newton, 0.22481, ForceUnit.PoundForce, 1e-5)]
        [InlineData(1, ForceUnit.Newton, 7.23301, ForceUnit.Poundal, 1e-5)]
        [InlineData(980665, ForceUnit.Dyne, 2.204622, ForceUnit.PoundForce, 1e-5)]
        public void Conversion(double value, ForceUnit unit, double expected, ForceUnit targetUnit, double accurracy = 1e-10)
        {
            var v = new Measurement<ForceUnit>(value, unit);
            v.In(targetUnit).Should().BeApproximately(expected, accurracy);
        }
    }
}
